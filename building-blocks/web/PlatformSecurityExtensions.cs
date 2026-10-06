using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Web;

/// <summary>
/// Cross-cutting OWASP Top 10 hardening applied by every HTTP service:
/// <list type="bullet">
///   <item>A01 Broken Access Control - deny-by-default fallback authorization policy.</item>
///   <item>A05 Security Misconfiguration - security headers, HSTS, config-driven CORS (never AllowAnyOrigin), ProblemDetails instead of stack traces.</item>
///   <item>A07 Identification and Authentication Failures - per-IP rate limiting with a stricter partition for sensitive endpoints.</item>
///   <item>A09 Logging and Monitoring Failures - rate-limit rejections and unhandled exceptions are logged.</item>
/// </list>
/// </summary>
public static class PlatformSecurityExtensions
{
    /// <summary>Name of the CORS policy built from the <c>Cors:AllowedOrigins</c> configuration key.</summary>
    public const string CorsPolicyName = "PlatformCors";

    /// <summary>Rate limit policy for sensitive endpoints (login, verification, machine ingestion).</summary>
    public const string SensitiveRateLimitPolicy = "platform-sensitive";

    /// <summary>
    /// Registers the platform-wide security defaults. Services keep calling
    /// <c>AddAuthorization(...)</c> themselves to declare their permission policies; the fallback
    /// policy registered here guarantees that any endpoint without explicit metadata requires an
    /// authenticated caller (A01).
    /// </summary>
    public static IServiceCollection AddPlatformSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPlatformAuthorization();
        services.AddPlatformRateLimiting(configuration);
        services.AddPlatformCors(configuration);
        services.AddProblemDetails();
        services.AddExceptionHandler<PlatformExceptionHandler>();
        return services;
    }

    /// <summary>
    /// A01: deny-by-default. Any endpoint that does not carry <c>[Authorize]</c> or
    /// <c>[AllowAnonymous]</c> requires a valid, authenticated JWT.
    /// </summary>
    public static IServiceCollection AddPlatformAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        return services;
    }

    /// <summary>
    /// A04/A07: global per-IP rate limit plus a stricter named policy
    /// (<see cref="SensitiveRateLimitPolicy"/>) for brute-forceable endpoints. Rejections are
    /// logged (A09) and answered with a generic 429 body.
    /// </summary>
    public static IServiceCollection AddPlatformRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var globalPermits = configuration.GetValue("RateLimit:GlobalPermitsPerMinute", 120);
        var sensitivePermits = configuration.GetValue("RateLimit:SensitivePermitsPerMinute", 10);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = globalPermits,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.AddPolicy(SensitiveRateLimitPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = sensitivePermits,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("PlatformSecurity");
                logger.LogWarning(
                    "Rate limit rejected {Method} {Path} from {RemoteIp}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    context.HttpContext.Connection.RemoteIpAddress);

                if (context.HttpContext.Response.HasStarted) return;

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new { error = "Too many requests. Please retry later." }, cancellationToken);
            };
        });

        return services;
    }

    /// <summary>
    /// A05: CORS is opt-in and restricted to explicitly configured origins. There is deliberately
    /// no <c>AllowAnyOrigin</c> fallback; when <c>Cors:AllowedOrigins</c> is absent the policy is
    /// not registered at all and the service stays same-origin only.
    /// </summary>
    public static IServiceCollection AddPlatformCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = ParseAllowedOrigins(configuration);
        if (origins.Length == 0) return services;

        services.AddCors(options => options.AddPolicy(CorsPolicyName, policyBuilder => policyBuilder
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(5))));

        return services;
    }

    /// <summary>Applies the configured CORS policy. No-op when no origins are configured.</summary>
    public static WebApplication UsePlatformCors(this WebApplication app)
    {
        if (ParseAllowedOrigins(app.Configuration).Length == 0) return app;
        app.UseCors(CorsPolicyName);
        return app;
    }

    /// <summary>
    /// A05: exception handling (generic ProblemDetails, full error logged server-side only) plus
    /// hardening response headers. Must be the first middleware of the pipeline.
    /// </summary>
    public static WebApplication UsePlatformSecurity(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
            headers["Cross-Origin-Opener-Policy"] = "same-origin";
            headers.Remove("X-Powered-By");
            await next();
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        return app;
    }

    private static string[] ParseAllowedOrigins(IConfiguration configuration)
    {
        var configured = configuration["Cors:AllowedOrigins"];
        if (string.IsNullOrWhiteSpace(configured)) return Array.Empty<string>();

        return configured.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}

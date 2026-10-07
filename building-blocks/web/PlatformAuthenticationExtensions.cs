using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BuildingBlocks.Web;

public static class PlatformAuthenticationExtensions
{
    public static IServiceCollection AddPlatformJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        if (string.IsNullOrWhiteSpace(secret) || Encoding.UTF8.GetByteCount(secret) < 32 ||
            string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Configure Jwt:Secret (at least 32 UTF-8 bytes), Jwt:Issuer and Jwt:Audience.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                // A09: record authentication failures, rejected authorizations and token errors so
                // brute force and token misuse attempts are visible to monitoring. Token material is
                // never logged.
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log(context.HttpContext, LogLevel.Warning,
                            "Token validation failed for {Method} {Path}: {Error}",
                            context.HttpContext.Request.Method, context.HttpContext.Request.Path,
                            context.Exception?.Message ?? "unknown error");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log(context.HttpContext, LogLevel.Information,
                            "Authentication challenge for {Method} {Path} from {RemoteIp}: {Reason}",
                            context.HttpContext.Request.Method, context.HttpContext.Request.Path,
                            context.HttpContext.Connection.RemoteIpAddress,
                            context.ErrorDescription ?? "missing or invalid credentials");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        var user = context.HttpContext.User;
                        // MapInboundClaims is disabled, so Identity.Name may be unset; fall back to
                        // the raw subject claim so the audit entry still identifies the caller.
                        var name = user?.Identity?.Name
                            ?? user?.FindFirst("unique_name")?.Value
                            ?? user?.FindFirst("sub")?.Value
                            ?? "unknown";
                        Log(context.HttpContext, LogLevel.Information,
                            "Access forbidden for user {User} on {Method} {Path}",
                            name,
                            context.HttpContext.Request.Method, context.HttpContext.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    private static void Log(HttpContext context, LogLevel level, string message, params object?[] args)
    {
        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("Platform.Authentication");
        logger.Log(level, message, args);
    }
}

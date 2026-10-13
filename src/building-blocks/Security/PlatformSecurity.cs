using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SmartTrade.Security;

public static class PlatformClaimTypes
{
    public const string TenantId = "tenant_id";
    public const string Subject = "sub";
    public const string Permission = "permissions";
    public const string DeviceId = "device_id";
}

public interface ITenantContext
{
    Guid? TenantId { get; }
    Guid? UserId { get; }
    Guid? DeviceId { get; }
}

public sealed class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    public Guid? TenantId => ReadGuid(PlatformClaimTypes.TenantId);
    public Guid? UserId => ReadGuid(PlatformClaimTypes.Subject);
    public Guid? DeviceId => ReadGuid(PlatformClaimTypes.DeviceId);

    private Guid? ReadGuid(string claimType) =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(claimType), out var value) ? value : null;
}

public sealed class PlatformAuthenticationOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
}

public static class PlatformSecurityRegistration
{
    public static IServiceCollection AddPlatformSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Authentication").Get<PlatformAuthenticationOptions>()
            ?? throw new InvalidOperationException("Authentication configuration is required.");
        if (string.IsNullOrWhiteSpace(settings.Issuer) || string.IsNullOrWhiteSpace(settings.Audience) ||
            string.IsNullOrWhiteSpace(settings.SigningKey) || System.Text.Encoding.UTF8.GetByteCount(settings.SigningKey) < 32)
            throw new InvalidOperationException("Authentication requires Issuer, Audience, and a SigningKey of at least 32 bytes.");

        services.Configure<PlatformAuthenticationOptions>(configuration.GetSection("Authentication"));
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<TenantRequiredFilter>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(settings.SigningKey)),
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                NameClaimType = PlatformClaimTypes.Subject,
                RoleClaimType = ClaimTypes.Role
            };
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });
        services.Configure<MvcOptions>(options => options.Filters.AddService<TenantRequiredFilter>());
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = false);
        return services;
    }
}

public sealed class TenantRequiredFilter : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity?.IsAuthenticated == true &&
            !user.IsInRole("platform_admin") &&
            !Guid.TryParse(user.FindFirstValue(PlatformClaimTypes.TenantId), out _))
        {
            context.Result = new ForbidResult();
        }
        return Task.CompletedTask;
    }
}

public sealed class PlatformExceptionMiddleware(RequestDelegate next, ILogger<PlatformExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(exception, "Request rejected by authorization or tenant boundary.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        catch (KeyNotFoundException exception)
        {
            logger.LogInformation(exception, "Requested record was not found.");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (ArgumentException exception)
        {
            logger.LogInformation(exception, "Request validation failed.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}

public static class PlatformExceptionHandling
{
    public static IApplicationBuilder UsePlatformExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<PlatformExceptionMiddleware>();
}

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) =>
        Task.FromResult<AuthorizationPolicy?>(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireClaim(PlatformClaimTypes.Permission, policyName)
            .Build());
}

public static class TenantControllerExtensions
{
    public static Guid GetRequiredTenantId(this ControllerBase controller) =>
        Guid.TryParse(controller.User.FindFirstValue(PlatformClaimTypes.TenantId), out var tenantId)
            ? tenantId
            : throw new UnauthorizedAccessException("A valid tenant_id claim is required.");

    public static Guid GetRequiredUserId(this ControllerBase controller) =>
        Guid.TryParse(controller.User.FindFirstValue(PlatformClaimTypes.Subject), out var userId)
            ? userId
            : throw new UnauthorizedAccessException("A valid sub claim is required.");
}

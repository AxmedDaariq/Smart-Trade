using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using BuildingBlocks.Web;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/identity/auth")]
// A01: authentication endpoints are the only anonymous surface of this service; every other
// endpoint falls back to "authenticated user required".
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private const string RefreshCookieName = "smarttrade_refresh";
    private const string RefreshCookiePath = "/api/v1/identity/auth";
    private readonly IIdentityAuthService _authService;

    public AuthController(IIdentityAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [EnableRateLimiting("identity-auth")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(command, ClientIpAddress(), Request.Headers["User-Agent"].ToString(), cancellationToken);
        if (result is null) return Unauthorized(new { error = "Invalid credentials or account unavailable." });
        if (result.RequiresMfa)
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "MFA challenge is required.", requiresMfa = true });

        SetRefreshCookie(result.RefreshToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("identity-auth")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken)) return Unauthorized();
        var result = await _authService.RefreshAsync(refreshToken, ClientIpAddress(), Request.Headers["User-Agent"].ToString(), cancellationToken);
        if (result is null)
        {
            ClearRefreshCookie();
            return Unauthorized();
        }
        SetRefreshCookie(result.RefreshToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("logout")]
    [EnableRateLimiting(PlatformSecurityExtensions.SensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken))
            await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        ClearRefreshCookie();
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(PlatformSecurityExtensions.SensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult ForgotPassword() =>
        StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "Password recovery is not configured." });

    private static LoginResponse ToResponse(LoginResult result) => new(
        result.AccessToken, result.ExpiresAtUtc, result.TenantType, result.Roles, result.Permissions, result.TenantCode);

    private void SetRefreshCookie(string token) => Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = RefreshCookiePath,
        IsEssential = true,
        MaxAge = TimeSpan.FromDays(30)
    });

    private void ClearRefreshCookie() => Response.Cookies.Delete(RefreshCookieName, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = RefreshCookiePath,
        IsEssential = true
    });

    private string ClientIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string TenantType,
        IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions, string TenantCode);
}

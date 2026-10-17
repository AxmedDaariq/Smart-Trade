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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginCommand? command, CancellationToken cancellationToken)
    {
        if (command is null || string.IsNullOrWhiteSpace(command.TenantCode) || string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
            return BadRequest(new { error = "Tenant code, email, and password are required." });

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest? request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { error = "Email is required." });

        await _authService.RequestPasswordResetAsync(request.Email, cancellationToken);
        return Ok(new { message = "If an account is associated with this email, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(PlatformSecurityExtensions.SensitiveRateLimitPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest? request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new { error = "Token and new password are required." });

        var success = await _authService.ResetPasswordAsync(request.Token, request.NewPassword, cancellationToken);
        if (!success) return BadRequest(new { error = "Invalid or expired reset token." });
        return Ok(new { message = "Password successfully reset." });
    }

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

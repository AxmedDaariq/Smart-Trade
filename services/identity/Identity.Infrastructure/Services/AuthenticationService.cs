using System.Security.Cryptography;
using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services;

public sealed class AuthenticationService(
    IdentityDbContext context,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthenticationService
{
    private static readonly TimeSpan RefreshLifetime = TimeSpan.FromDays(30);

    public async Task<LoginResult?> LoginAsync(LoginCommand command, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var tenantCode = command.TenantCode.Trim().ToUpperInvariant();
        var tenant = await context.Tenants.SingleOrDefaultAsync(t => t.Code == tenantCode && t.Status == "Active", cancellationToken);
        if (tenant is null) return null;

        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(
            u => u.TenantId == tenant.Id && u.NormalizedEmail == normalizedEmail, cancellationToken);
        if (user is null || user.Status != "Active" || user.LockoutEnd > DateTime.UtcNow) return null;

        if (!passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5) user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            user.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            return null;
        }

        if (user.MfaEnabled) return new LoginResult { RequiresMfa = true };
        if (!user.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal))
            user.PasswordHash = passwordHasher.HashPassword(command.Password);

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        var permissions = await (from userRole in context.UserRoles.IgnoreQueryFilters()
                                 join role in context.Roles.IgnoreQueryFilters() on userRole.RoleId equals role.Id
                                 join rolePermission in context.RolePermissions.IgnoreQueryFilters() on role.Id equals rolePermission.RoleId
                                 join permission in context.Permissions on rolePermission.PermissionId equals permission.Id
                                 where userRole.UserId == user.Id && role.TenantId == tenant.Id
                                 select permission.Code).Distinct().ToArrayAsync(cancellationToken);

        var refreshToken = tokenGenerator.GenerateRefreshToken();
        var session = new Session
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            RefreshTokenHash = tokenGenerator.HashToken(refreshToken),
            IPAddress = ipAddress,
            UserAgent = userAgent,
            ExpiresAt = DateTime.UtcNow.Add(RefreshLifetime)
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync(cancellationToken);

        return new LoginResult
        {
            SessionId = session.Id,
            AccessToken = tokenGenerator.GenerateAccessToken(user, tenant, permissions),
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<LoginResult?> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var session = await context.Sessions.IgnoreQueryFilters().SingleOrDefaultAsync(
            s => s.Id == command.SessionId, cancellationToken);
        if (session is null || session.RevokedAt != null || session.ExpiresAt <= DateTime.UtcNow) return null;

        var submittedHash = tokenGenerator.HashToken(command.RefreshToken);
        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(submittedHash), Convert.FromBase64String(session.RefreshTokenHash)))
        {
            session.RevokedAt = DateTime.UtcNow;
            session.RevocationReason = "Refresh token reuse or invalid token";
            await context.SaveChangesAsync(cancellationToken);
            return null;
        }

        var user = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(
            u => u.Id == session.UserId && u.TenantId == session.TenantId && u.Status == "Active", cancellationToken);
        var tenant = await context.Tenants.SingleOrDefaultAsync(
            t => t.Id == session.TenantId && t.Status == "Active", cancellationToken);
        if (user is null || tenant is null) return null;

        var permissions = await (from userRole in context.UserRoles.IgnoreQueryFilters()
                                 join role in context.Roles.IgnoreQueryFilters() on userRole.RoleId equals role.Id
                                 join rolePermission in context.RolePermissions.IgnoreQueryFilters() on role.Id equals rolePermission.RoleId
                                 join permission in context.Permissions on rolePermission.PermissionId equals permission.Id
                                 where userRole.UserId == user.Id && role.TenantId == tenant.Id
                                 select permission.Code).Distinct().ToArrayAsync(cancellationToken);
        var newRefreshToken = tokenGenerator.GenerateRefreshToken();
        session.RefreshTokenHash = tokenGenerator.HashToken(newRefreshToken);
        session.LastUsedAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.Add(RefreshLifetime);
        await context.SaveChangesAsync(cancellationToken);

        return new LoginResult
        {
            SessionId = session.Id,
            AccessToken = tokenGenerator.GenerateAccessToken(user, tenant, permissions),
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }

    public async Task<bool> RevokeAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken)
    {
        var session = await context.Sessions.SingleOrDefaultAsync(
            s => s.Id == sessionId && s.UserId == userId, cancellationToken);
        if (session is null) return false;
        session.RevokedAt = DateTime.UtcNow;
        session.RevocationReason = "User signed out";
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

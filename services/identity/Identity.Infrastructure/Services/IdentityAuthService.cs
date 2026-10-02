using Identity.Application.Commands.Auth;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Identity.Infrastructure.Services;

public sealed class IdentityAuthService : IIdentityAuthService
{
    private const int MaximumFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly string DummyPasswordHash = new Pbkdf2PasswordHasher().HashPassword(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
    private readonly IdentityDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokens;
    private readonly JwtTokenOptions _jwtOptions;

    public IdentityAuthService(IdentityDbContext db, IPasswordHasher passwordHasher, IJwtTokenGenerator tokens,
        IOptions<JwtTokenOptions> jwtOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResult?> LoginAsync(LoginCommand command, string ipAddress, string userAgent, CancellationToken cancellationToken)
    {
        var normalizedTenantCode = command.TenantCode.Trim().ToUpperInvariant();
        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        var tenant = await _db.Tenants.AsNoTracking()
            .SingleOrDefaultAsync(t => t.TenantCode.ToUpper() == normalizedTenantCode, cancellationToken);
        var user = tenant is null ? null : await _db.Users
            .SingleOrDefaultAsync(u => u.TenantId == tenant.Id && u.NormalizedEmail == normalizedEmail, cancellationToken);

        var passwordValid = _passwordHasher.VerifyPassword(command.Password, user?.PasswordHash ?? DummyPasswordHash);
        if (user is null || tenant is null || !passwordValid || tenant.Status != "Active" || user.Status != "Active")
        {
            if (user is not null && !passwordValid)
            {
                user.FailedLoginCount++;
                user.LastFailedLoginAtUtc = DateTime.UtcNow;
                if (user.FailedLoginCount >= MaximumFailedAttempts)
                    user.LockedUntilUtc = DateTime.UtcNow.Add(LockoutDuration);
                await _db.SaveChangesAsync(cancellationToken);
            }
            return null;
        }

        var now = DateTime.UtcNow;
        if (user.LockedUntilUtc > now) return null;
        if (!user.EmailVerified) return null;
        if (user.MfaEnabled)
            return new LoginResult { RequiresMfa = true };

        user.FailedLoginCount = 0;
        user.LockedUntilUtc = null;
        user.LastLoginAtUtc = now;
        var authorization = await LoadAuthorizationAsync(user.Id, tenant.Id, cancellationToken);
        return await CreateSessionAsync(user, tenant, authorization.Roles, authorization.Permissions, ipAddress, userAgent, now, cancellationToken);
    }

    public async Task<LoginResult?> RefreshAsync(string refreshToken, string ipAddress, string userAgent, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;
        var now = DateTime.UtcNow;
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var tokenHash = _tokens.HashToken(refreshToken);
        var current = await _db.RefreshTokens.Include(t => t.Session)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        if (current is null) return null;

        if (current.Status != "Active" || current.UsedAtUtc is not null || current.RevokedAtUtc is not null)
        {
            if (current.UsedAtUtc is not null)
            {
                await RevokeFamilyAsync(current.TokenFamilyId, current.SessionId, now, "Refresh token reuse detected", cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            return null;
        }
        if (current.ExpiresAtUtc <= now || current.Session.ExpiresAtUtc <= now || current.Session.RevokedAtUtc is not null)
            return null;

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == current.UserId, cancellationToken);
        var tenant = user is null ? null : await _db.Tenants.SingleOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken);
        if (user is null || tenant is null || user.Status != "Active" || tenant.Status != "Active" || user.MfaEnabled)
            return null;

        var authorization = await LoadAuthorizationAsync(user.Id, tenant.Id, cancellationToken);
        current.Status = "Used";
        current.UsedAtUtc = now;
        var (next, plainNextToken) = NewRefreshToken(user, current.Session, current.TokenFamilyId, ipAddress, userAgent, now);
        current.ReplacedByTokenId = next.Id;
        _db.RefreshTokens.Add(next);
        current.Session.LastSeenAtUtc = now;
        user.LastLoginAtUtc = now;
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
        await transaction.CommitAsync(cancellationToken);

        return new LoginResult
        {
            AccessToken = _tokens.GenerateAccessToken(user, tenant, authorization.Permissions),
            RefreshToken = plainNextToken,
            ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes),
            TenantCode = tenant.TenantCode,
            TenantType = tenant.TenantType,
            Roles = authorization.Roles,
            Permissions = authorization.Permissions
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return;
        var tokenHash = _tokens.HashToken(refreshToken);
        var token = await _db.RefreshTokens.Include(t => t.Session)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        if (token is null || token.RevokedAtUtc is not null) return;
        await RevokeFamilyAsync(token.TokenFamilyId, token.SessionId, DateTime.UtcNow, "User signed out", cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<LoginResult> CreateSessionAsync(User user, Tenant tenant, IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions, string ipAddress, string userAgent, DateTime now, CancellationToken cancellationToken)
    {
        var expiresAt = now.AddDays(30);
        var session = new Session
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            SessionReference = Guid.NewGuid().ToString("N"),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAtUtc = now,
            LastSeenAtUtc = now,
            ExpiresAtUtc = expiresAt,
            CorrelationId = Guid.NewGuid().ToString("N")
        };
        var (refreshToken, plainRefreshToken) = NewRefreshToken(user, session, Guid.NewGuid(), ipAddress, userAgent, now);
        _db.Sessions.Add(session);
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);
        return new LoginResult
        {
            AccessToken = _tokens.GenerateAccessToken(user, tenant, permissions),
            RefreshToken = plainRefreshToken,
            ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes),
            TenantCode = tenant.TenantCode,
            TenantType = tenant.TenantType,
            Roles = roles,
            Permissions = permissions
        };
    }

    private (RefreshToken Token, string PlainToken) NewRefreshToken(User user, Session session, Guid familyId, string ipAddress, string userAgent, DateTime now)
    {
        var plainToken = _tokens.GenerateRefreshToken();
        return (new RefreshToken
        {
            SessionId = session.Id,
            UserId = user.Id,
            TokenHash = _tokens.HashToken(plainToken),
            TokenFamilyId = familyId,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(30),
            CreatedFromIp = ipAddress,
            CreatedFromUserAgent = userAgent,
            Status = "Active"
        }, plainToken);
    }

    private async Task<(IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions)> LoadAuthorizationAsync(
        Guid userId, Guid tenantId, CancellationToken cancellationToken)
    {
        var assignments = await _db.UserRoles.AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.Status == "Active" && (ur.ExpiresAtUtc == null || ur.ExpiresAtUtc > DateTime.UtcNow)
                && ur.Role.Status == "Active" && (ur.Role.TenantId == null || ur.Role.TenantId == tenantId))
            .Select(ur => new
            {
                ur.Role.Name,
                Permissions = ur.Role.RolePermissions
                    .Where(rp => rp.Status == "Active")
                    .Select(rp => rp.Permission.Code)
            })
            .ToListAsync(cancellationToken);

        return (assignments.Select(a => a.Name).Distinct(StringComparer.Ordinal).ToArray(),
            assignments.SelectMany(a => a.Permissions).Distinct(StringComparer.Ordinal).ToArray());
    }

    private async Task RevokeFamilyAsync(Guid familyId, Guid sessionId, DateTime now, string reason, CancellationToken cancellationToken)
    {
        var family = await _db.RefreshTokens.Where(t => t.TokenFamilyId == familyId && t.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        foreach (var token in family)
        {
            token.Status = "Revoked";
            token.RevokedAtUtc = now;
            token.RevocationReason = reason;
        }
        var session = await _db.Sessions.SingleOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        if (session is not null)
        {
            session.Status = "Revoked";
            session.RevokedAtUtc = now;
            session.RevocationReason = reason;
        }
    }
}

using System.Text;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Seeds the Identity database with tenants, users, roles and permissions
/// for integration tests. Uses the real PBKDF2 password hasher so seeded
/// credentials behave exactly like production ones.
/// </summary>
public static class IdentityTestData
{
    public const string TestPassword = "TestPassword123!";

    public static async Task<(Guid tenantId, Guid userId, string tenantCode)> SeedActiveUserAsync(
        string connectionString,
        string tenantCode,
        string email,
        string[] permissions,
        bool emailVerified = true,
        bool mfaEnabled = false,
        string userStatus = "Active",
        string tenantStatus = "Active")
    {
        await using var ctx = CreateContext(connectionString);
        var hasher = new Pbkdf2PasswordHasher();

        var tenant = new Tenant
        {
            TenantCode = tenantCode,
            Name = $"Tenant {tenantCode}",
            LegalName = $"Tenant {tenantCode} Ltd",
            TenantType = "LogisticsCompany",
            Status = tenantStatus,
            CountryCode = "SO",
            TimeZone = "Africa/Mogadishu",
            DefaultCurrency = "USD",
            ContactEmail = $"admin@{tenantCode}.example.com",
            CreatedBy = Guid.NewGuid()
        };
        ctx.Tenants.Add(tenant);

        var user = new User
        {
            TenantId = tenant.Id,
            Username = email.Split('@')[0],
            NormalizedUsername = email.Split('@')[0].ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailVerified = emailVerified,
            PasswordHash = hasher.HashPassword(TestPassword),
            PasswordAlgorithm = "pbkdf2-sha256",
            FirstName = "Test",
            LastName = "User",
            DisplayName = "Test User",
            Status = userStatus,
            MfaEnabled = mfaEnabled,
            CreatedBy = Guid.NewGuid()
        };
        ctx.Users.Add(user);

        // Role + permissions
        var role = new Role
        {
            Name = "TestRole",
            NormalizedName = "TESTROLE",
            RoleType = "Custom",
            Status = "Active",
            CreatedBy = Guid.NewGuid()
        };
        ctx.Roles.Add(role);

        foreach (var permission in permissions)
        {
            var perm = await ctx.Permissions
                .FirstOrDefaultAsync(p => p.Code == permission);
            if (perm is null)
            {
                perm = new Permission
                {
                    Code = permission,
                    Name = permission,
                    Description = $"Test permission {permission}",
                    Resource = "test",
                    Action = "test"
                };
                ctx.Permissions.Add(perm);
            }

            ctx.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = perm.Id,
                AssignedBy = Guid.NewGuid()
            });
        }

        ctx.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedBy = Guid.NewGuid()
        });

        await ctx.SaveChangesAsync();
        return (tenant.Id, user.Id, tenant.TenantCode);
    }

    public static IdentityDbContext CreateContext(string connectionString) =>
        new(new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString)
            .Options);

    /// <summary>Hashes a refresh token the same way the service does (SHA-256).</summary>
    public static string HashRefreshToken(string plainToken)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return Convert.ToBase64String(
            sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken)));
    }

    /// <summary>Extracts the plain refresh token value from a Set-Cookie header.
    /// The token is base64 and may contain '=' padding, so only the first
    /// '=' is treated as the name/value separator.</summary>
    public static string ExtractRefreshToken(HttpResponseMessage loginResponse)
    {
        var cookie = loginResponse.Headers.GetValues("Set-Cookie")
            .First(c => c.StartsWith("smarttrade_refresh="));
        var pair = cookie.Split(';')[0];
        var separator = pair.IndexOf('=');
        return pair[(separator + 1)..];
    }
}

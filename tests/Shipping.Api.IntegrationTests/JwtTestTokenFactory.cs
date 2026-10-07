using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shipping.Api.IntegrationTests;

/// <summary>
/// Creates JWTs signed with the same key the test host is configured with,
/// so authentication and authorization can be exercised end-to-end.
/// Claims mirror what the identity service issues: <c>sub</c> (user id),
/// <c>tenant_id</c> (tenant isolation comes from the token, never the
/// query string — A01) and <c>permissions</c> (one claim per permission).
/// </summary>
public static class JwtTestTokenFactory
{
    public const string Secret = "integration-test-signing-key-with-at-least-32-bytes!";
    public const string Issuer = "smart-trade-integration-tests";
    public const string Audience = "smart-trade-integration-tests";

    public static string Generate(
        Guid tenantId,
        Guid userId,
        IEnumerable<string> permissions,
        TimeSpan? lifetime = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("tenant_id", tenantId.ToString())
        };
        claims.AddRange(permissions.Select(p => new Claim("permissions", p)));

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromHours(1)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Produces a valid token that carries no <c>tenant_id</c> claim.</summary>
    public static string GenerateWithoutTenant(Guid userId, IEnumerable<string> permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(permissions.Select(p => new Claim("permissions", p)));

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Produces a token whose signature no longer matches the key.</summary>
    public static string Tamper(string validToken)
    {
        var parts = validToken.Split('.');
        if (parts.Length != 3) throw new ArgumentException("Not a JWT", nameof(validToken));
        // Flip the last character of the signature segment.
        var sig = parts[2].ToCharArray();
        sig[^1] = sig[^1] == 'a' ? 'b' : 'a';
        return $"{parts[0]}.{parts[1]}.{new string(sig)}";
    }
}

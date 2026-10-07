using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Creates JWTs signed with the same key the Identity test host is
/// configured with, so authorization policies can be exercised
/// end-to-end. Claims mirror what the identity service issues:
/// <c>sub</c> (user id), <c>tenant_id</c>, and <c>permissions</c>.
/// </summary>
public static class IdentityJwtTestTokenFactory
{
    public const string Secret = "identity-integration-test-signing-key-32bytes!";
    public const string Issuer = "smart-trade-identity-it";
    public const string Audience = "smart-trade-identity-it";

    public static string Generate(Guid tenantId, Guid userId, IEnumerable<string> permissions)
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
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey = "SUPER_SECRET_KEY_THAT_SHOULD_BE_IN_VAULT_NOT_HERE";
    private readonly string _issuer = "SmartTradePlatform";
    private readonly string _audience = "SmartTradeServices";

    public string GenerateAccessToken(User user, Tenant tenant, IEnumerable<string> permissions)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("tenant_id", tenant.Id.ToString()),
            new Claim("tenant_code", tenant.Code)
        };

        if (user.OrganizationId.HasValue)
        {
            claims.Add(new Claim("organization_id", user.OrganizationId.Value.ToString()));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Short-lived
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}

using Identity.Domain.Entities;
using Identity.Infrastructure.Security;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace Identity.UnitTests;

public sealed class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateAccessToken_UsesConfiguredIssuerAudienceAndTenantClaims()
    {
        var generator = new JwtTokenGenerator(Options.Create(new JwtTokenOptions
        {
            Secret = Convert.ToBase64String(new byte[48]),
            Issuer = "https://identity.example.test",
            Audience = "smart-trade-services",
            AccessTokenLifetimeMinutes = 12
        }));
        var user = new User { Id = Guid.NewGuid(), Email = "user@example.test" };
        var tenant = new Tenant { Id = Guid.NewGuid(), TenantCode = "TENANT-1" };

        var token = new JwtSecurityTokenHandler().ReadJwtToken(generator.GenerateAccessToken(user, tenant, ["ectn.read"]));

        Assert.Equal("https://identity.example.test", token.Issuer);
        Assert.Contains("smart-trade-services", token.Audiences);
        Assert.Equal(tenant.Id.ToString(), token.Claims.Single(c => c.Type == "tenant_id").Value);
        Assert.Equal("TENANT-1", token.Claims.Single(c => c.Type == "tenant_code").Value);
        Assert.Contains(token.Claims, c => c.Type == "permissions" && c.Value == "ectn.read");
        Assert.Equal(TimeSpan.FromMinutes(12), token.ValidTo - token.ValidFrom);
    }

    [Fact]
    public void Constructor_RejectsSigningKeyShorterThan256Bits()
    {
        Assert.Throws<InvalidOperationException>(() => new JwtTokenGenerator(Options.Create(new JwtTokenOptions
        {
            Secret = "short",
            Issuer = "issuer",
            Audience = "audience"
        })));
    }
}

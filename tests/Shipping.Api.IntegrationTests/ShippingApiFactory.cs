using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Shipping.Api.IntegrationTests;

/// <summary>
/// Boots the real Shipping.Api pipeline against the shared PostgreSQL
/// Testcontainer. Configuration is overridden in-memory (highest priority)
/// so the service never touches a real database or secret store:
/// - ConnectionStrings:ShippingDb → the Testcontainer PostgreSQL
/// - Jwt:* → a fixed test signing key (A02: ≥32 UTF-8 bytes, validated at startup)
/// - RateLimit:* → raised so the test suite is never throttled (A04/A07)
/// </summary>
public sealed class ShippingApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ShippingApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ShippingDb"] = _connectionString,
                ["Jwt:Secret"] = JwtTestTokenFactory.Secret,
                ["Jwt:Issuer"] = JwtTestTokenFactory.Issuer,
                ["Jwt:Audience"] = JwtTestTokenFactory.Audience,
                ["RateLimit:GlobalPermitsPerMinute"] = "10000",
                ["RateLimit:SensitivePermitsPerMinute"] = "10000",
                // No CORS origins: the service must stay same-origin only (A05).
                ["Cors:AllowedOrigins"] = ""
            });
        });
    }
}

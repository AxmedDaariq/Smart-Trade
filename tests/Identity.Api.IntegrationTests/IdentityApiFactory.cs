using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Boots the real Identity.Api pipeline against the shared PostgreSQL
/// Testcontainer. Configuration is overridden in-memory (highest priority)
/// so the service never touches a real database or secret store:
/// - ConnectionStrings:DefaultConnection → the Testcontainer PostgreSQL
/// - Jwt:* → a fixed test signing key (A02: ≥32 UTF-8 bytes, validated at startup)
/// - RateLimit:* → raised so the test suite is never throttled (A04/A07)
/// </summary>
public sealed class IdentityApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public IdentityApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Jwt:Secret"] = "identity-integration-test-signing-key-32bytes!",
                ["Jwt:Issuer"] = "smart-trade-identity-it",
                ["Jwt:Audience"] = "smart-trade-identity-it",
                ["Jwt:AccessTokenLifetimeMinutes"] = "15",
                ["RateLimit:GlobalPermitsPerMinute"] = "10000",
                ["RateLimit:SensitivePermitsPerMinute"] = "10000",
                // No CORS origins: the service must stay same-origin only (A05).
                ["Cors:AllowedOrigins"] = ""
            });
        });
    }
}

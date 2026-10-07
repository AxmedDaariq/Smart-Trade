using System.Net;
using System.Net.Http.Headers;
using Identity.Api.IntegrationTests;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Integration tests for health endpoints, security headers, and CORS
/// configuration — the platform-wide hardening applied by
/// PlatformSecurityExtensions (A05).
/// </summary>
[Collection("identity-postgres")]
public class HealthAndSecurityIntegrationTests
{
    private readonly IdentityPostgresFixture _db;
    private readonly HttpClient _client;

    public HealthAndSecurityIntegrationTests(IdentityPostgresFixture db)
    {
        _db = db;
        var factory = new IdentityApiFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    // ── Health ─────────────────────────────────────────────────────

    [Fact]
    public async Task HealthLive_WithoutToken_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthReady_WithoutToken_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Security headers (A05) ────────────────────────────────────

    [Fact]
    public async Task SecurityHeaders_ArePresentOnResponses()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").First());
        Assert.Contains("camera=()", response.Headers.GetValues("Permissions-Policy").First());
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Opener-Policy").First());
        Assert.DoesNotContain("X-Powered-By", response.Headers);
    }

    // ── CORS (A05: opt-in, no AllowAnyOrigin) ─────────────────────

    [Fact]
    public async Task Cors_UnconfiguredOrigin_DoesNotGetAllowHeaders()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/identity/auth/login");
        request.Headers.Add("Origin", "https://evil.example.com");
        request.Headers.Add("Access-Control-request-method", "POST");

        var response = await _client.SendAsync(request);

        // No CORS origins configured → no Access-Control-Allow-* headers.
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.False(response.Headers.Contains("Access-Control-Allow-Methods"));
    }

    // ── Deny-by-default (A01) ─────────────────────────────────────

    [Fact]
    public async Task AnyEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        // Users and Tenants controllers require auth; the fallback policy
        // protects everything without explicit [AllowAnonymous].
        var usersResponse = await _client.GetAsync("/api/v1/identity/users");
        var tenantsResponse = await _client.GetAsync("/api/v1/identity/tenants");

        Assert.Equal(HttpStatusCode.Unauthorized, usersResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, tenantsResponse.StatusCode);
    }
}

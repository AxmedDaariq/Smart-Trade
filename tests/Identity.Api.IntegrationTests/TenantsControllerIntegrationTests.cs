using System.Net;
using Identity.Api.IntegrationTests;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Integration tests for TenantsController — authorization (deny-by-default,
/// permission policies) and the 501 stub responses.
/// </summary>
[Collection("identity-postgres")]
public class TenantsControllerIntegrationTests
{
    private readonly IdentityPostgresFixture _db;
    private readonly HttpClient _client;

    public TenantsControllerIntegrationTests(IdentityPostgresFixture db)
    {
        _db = db;
        var factory = new IdentityApiFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private void Authenticate(string[] permissions)
    {
        var token = IdentityJwtTestTokenFactory.Generate(
            Guid.NewGuid(), Guid.NewGuid(), permissions);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    [Fact]
    public async Task GetTenants_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/identity/tenants");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTenants_WithoutPermission_ReturnsForbidden()
    {
        Authenticate(Array.Empty<string>());

        var response = await _client.GetAsync("/api/v1/identity/tenants");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetTenants_WithPermission_ReturnsNotImplemented()
    {
        Authenticate(new[] { "identity.tenants.read" });

        var response = await _client.GetAsync("/api/v1/identity/tenants");

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [Fact]
    public async Task CreateTenant_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/v1/identity/tenants", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTenant_WithoutPermission_ReturnsForbidden()
    {
        Authenticate(new[] { "identity.tenants.read" });

        var response = await _client.PostAsync("/api/v1/identity/tenants", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateTenant_WithPermission_ReturnsNotImplemented()
    {
        Authenticate(new[] { "identity.tenants.manage" });

        var response = await _client.PostAsync("/api/v1/identity/tenants", null);

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [Fact]
    public async Task SuspendTenant_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync($"/api/v1/identity/tenants/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SuspendTenant_WithoutPermission_ReturnsForbidden()
    {
        Authenticate(new[] { "identity.tenants.read" });

        var response = await _client.PostAsync($"/api/v1/identity/tenants/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SuspendTenant_WithPermission_ReturnsNotImplemented()
    {
        Authenticate(new[] { "identity.tenants.manage" });

        var response = await _client.PostAsync($"/api/v1/identity/tenants/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}

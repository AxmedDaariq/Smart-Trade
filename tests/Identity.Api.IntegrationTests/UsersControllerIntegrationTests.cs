using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Identity.Api.IntegrationTests;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Integration tests for UsersController — authorization (deny-by-default,
/// permission policies) and the 501 stub responses.
/// </summary>
[Collection("identity-postgres")]
public class UsersControllerIntegrationTests
{
    private readonly IdentityPostgresFixture _db;
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests(IdentityPostgresFixture db)
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
    public async Task GetUsers_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/identity/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithoutPermission_ReturnsForbidden()
    {
        Authenticate(Array.Empty<string>());

        var response = await _client.GetAsync("/api/v1/identity/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithPermission_ReturnsNotImplemented()
    {
        Authenticate(new[] { "identity.users.read" });

        var response = await _client.GetAsync("/api/v1/identity/users");

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [Fact]
    public async Task SuspendUser_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync($"/api/v1/identity/users/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SuspendUser_WithoutPermission_ReturnsForbidden()
    {
        Authenticate(new[] { "identity.users.read" });

        var response = await _client.PostAsync($"/api/v1/identity/users/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SuspendUser_WithPermission_ReturnsNotImplemented()
    {
        Authenticate(new[] { "identity.users.manage" });

        var response = await _client.PostAsync($"/api/v1/identity/users/{Guid.NewGuid()}/suspend", null);

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}

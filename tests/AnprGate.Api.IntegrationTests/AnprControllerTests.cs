using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AnprGate.Api.IntegrationTests;

/// <summary>
/// Boots the real API pipeline. The service fails fast when no signing key is configured (A02),
/// so the tests inject their own JWT configuration through environment variables - the only
/// configuration source that is guaranteed to be visible while <c>Program.cs</c> runs.
/// </summary>
public sealed class AnprApiFactory : WebApplicationFactory<Program>
{
    public AnprApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Secret", "integration-test-signing-key-with-at-least-32-bytes!");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "smart-trade-integration-tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "smart-trade-integration-tests");
    }
}

public class AnprControllerTests : IClassFixture<AnprApiFactory>
{
    private readonly AnprApiFactory _factory;

    public AnprControllerTests(AnprApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostIngest_WithoutAuth_ShouldReturn_Unauthorized()
    {
        // Arrange: HttpClient macmal ah oo aan wadan wax Token ah
        var client = _factory.CreateClient();
        var content = new StringContent("{\"plateNumber\": \"SO-1234\"}", Encoding.UTF8, "application/json");

        // Act: Diritaanka POST request
        var response = await client.PostAsync("/api/v1/anpr/ingest", content);

        // Assert: Qalabka ANPR waa inuu wataa Token, haddii kale API-gu waa inuu diidaa (401)
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetIngest_ShouldReturn_MethodNotAllowed_Or_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act: Diritaanka GET request (halkii POST laga rabo)
        var response = await client.GetAsync("/api/v1/anpr/ingest");

        // Assert: Endpoint-kani POST kaliya ayuu qaataa
        Assert.True(response.StatusCode == HttpStatusCode.MethodNotAllowed ||
                    response.StatusCode == HttpStatusCode.NotFound ||
                    response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthLive_WithoutAuth_ShouldRemainReachable()
    {
        // Arrange: k8s probes must never depend on a token (A01 fallback policy exception)
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

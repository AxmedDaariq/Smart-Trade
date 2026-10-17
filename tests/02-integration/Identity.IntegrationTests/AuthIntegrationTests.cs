using System.Net;
using FluentAssertions;
using Shared.IntegrationTests;
using Xunit;

namespace Identity.IntegrationTests;

public class AuthIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _containerFixture;
    private readonly IdentityApiFactory _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _factory = new IdentityApiFactory(_containerFixture);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthReady_ShouldReturnOk_WhenDatabaseIsUp()
    {
        // Arrange
        await _factory.MigrateDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthLive_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

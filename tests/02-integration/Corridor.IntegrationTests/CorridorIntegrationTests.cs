using System.Net;
using FluentAssertions;
using Shared.IntegrationTests;
using Xunit;

namespace Corridor.IntegrationTests;

public class CorridorIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _containerFixture;
    private readonly CorridorApiFactory _factory;
    private readonly HttpClient _client;

    public CorridorIntegrationTests(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _factory = new CorridorApiFactory(_containerFixture);
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
}

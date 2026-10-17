using System.Net;
using FluentAssertions;
using Shared.IntegrationTests;
using Xunit;

namespace AnprGate.IntegrationTests;

public class AnprGateIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _containerFixture;
    private readonly AnprGateApiFactory _factory;
    private readonly HttpClient _client;

    public AnprGateIntegrationTests(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _factory = new AnprGateApiFactory(_containerFixture);
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

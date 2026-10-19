using System.Net;
using FluentAssertions;
using Shared.IntegrationTests;
using Xunit;

namespace GpsTelematics.IntegrationTests;

public class GpsTelematicsIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _containerFixture;
    private readonly GpsTelematicsApiFactory _factory;
    private readonly HttpClient _client;

    public GpsTelematicsIntegrationTests(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _factory = new GpsTelematicsApiFactory(_containerFixture);
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

using System.Net;
using FluentAssertions;
using Shared.IntegrationTests;
using Xunit;

namespace Payment.IntegrationTests;

public class PaymentIntegrationTests : IClassFixture<ContainerFixture>
{
    private readonly ContainerFixture _containerFixture;
    private readonly PaymentApiFactory _factory;
    private readonly HttpClient _client;

    public PaymentIntegrationTests(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
        _factory = new PaymentApiFactory(_containerFixture);
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

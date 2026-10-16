using System;
using System.Threading;
using System.Threading.Tasks;
using Analytics.Application.Commands;
using Analytics.Application.Interfaces;
using Analytics.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Analytics.UnitTests.Services;

public class GenerateReportCommandHandlerTests
{
    private readonly Mock<IAnalyticsDbContext> _mockContext;
    private readonly GenerateReportCommandHandler _handler;

    public GenerateReportCommandHandlerTests()
    {
        _mockContext = new Mock<IAnalyticsDbContext>();
        _handler = new GenerateReportCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenReportDefinitionNotFound()
    {
        // Arrange
        var command = new GenerateReportCommand
        {
            ReportDefinitionId = Guid.NewGuid(),
            RequestingTenantId = Guid.NewGuid()
        };

        _mockContext
            .Setup(c => c.FindReportDefinitionAsync(command.ReportDefinitionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReportDefinition?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenTenantMismatches()
    {
        // Arrange
        var definitionTenantId = Guid.NewGuid();
        var differentTenantId = Guid.NewGuid();

        var definition = new ReportDefinition(
            tenantId: definitionTenantId,
            name: "Weekly Corridor Report",
            type: "Corridor",
            format: "PDF",
            scheduleCron: "0 0 * * 0");

        var command = new GenerateReportCommand
        {
            ReportDefinitionId = definition.Id,
            RequestingTenantId = differentTenantId
        };

        _mockContext
            .Setup(c => c.FindReportDefinitionAsync(definition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(definition);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*different tenant*");
    }

    [Fact]
    public async Task Handle_ShouldReturnSignedUrl_WhenTenantMatches()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var definition = new ReportDefinition(
            tenantId: tenantId,
            name: "Monthly Financial KPI Report",
            type: "Executive",
            format: "PDF",
            scheduleCron: "0 0 1 * *");

        var command = new GenerateReportCommand
        {
            ReportDefinitionId = definition.Id,
            RequestingTenantId = tenantId
        };

        _mockContext
            .Setup(c => c.FindReportDefinitionAsync(definition.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(definition);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith("https://");
    }
}

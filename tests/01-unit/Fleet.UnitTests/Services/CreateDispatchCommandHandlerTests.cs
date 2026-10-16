using System;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Application.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace Fleet.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CreateDispatchCommandHandler"/>.
/// Validates dispatch creation logic using mocked persistence.
/// </summary>
public sealed class CreateDispatchCommandHandlerTests
{
    private readonly Mock<IFleetDbContext> _contextMock = new();

    [Fact]
    public async Task Handle_ValidCommand_CreatesDispatchAndReturnsId()
    {
        // Arrange
        _contextMock
            .Setup(c => c.DispatchOrders.Add(It.IsAny<Fleet.Domain.Entities.DispatchOrder>()));
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateDispatchCommandHandler(_contextMock.Object);
        var command = new CreateDispatchCommand
        {
            TenantId = Guid.NewGuid(),
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            CargoReference = "CARGO-001",
            CustomsCaseReference = "CUS-2026-001"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty("the handler should return a new dispatch ID");
        _contextMock.Verify(c => c.DispatchOrders.Add(It.Is<Fleet.Domain.Entities.DispatchOrder>(d =>
            d.TenantId == command.TenantId &&
            d.FleetCompanyId == command.FleetCompanyId &&
            d.VehicleId == command.VehicleId &&
            d.DriverId == command.DriverId &&
            d.CargoReference == "CARGO-001" &&
            d.CustomsReleaseReference == "CUS-2026-001" &&
            d.Status == "Draft" &&
            d.Priority == "Normal" &&
            d.DispatchReference.StartsWith("DSP-")
        )), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_GeneratesUniqueDispatchReference()
    {
        // Arrange
        var capturedDispatches = new System.Collections.Generic.List<Fleet.Domain.Entities.DispatchOrder>();
        _contextMock
            .Setup(c => c.DispatchOrders.Add(It.IsAny<Fleet.Domain.Entities.DispatchOrder>()))
            .Callback<Fleet.Domain.Entities.DispatchOrder>(d => capturedDispatches.Add(d));
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateDispatchCommandHandler(_contextMock.Object);
        var command = new CreateDispatchCommand
        {
            TenantId = Guid.NewGuid(),
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        // Act
        var id1 = await handler.Handle(command, CancellationToken.None);
        var id2 = await handler.Handle(command, CancellationToken.None);

        // Assert – different calls should produce different IDs and references
        id1.Should().NotBe(id2);
        capturedDispatches.Should().HaveCount(2);
        capturedDispatches[0].DispatchReference.Should().NotBe(capturedDispatches[1].DispatchReference);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Application.Commands;
using Fleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fleet.Tests.Application.Commands;

public class CreateDispatchCommandHandlerTests
{
    private readonly Mock<IFleetDbContext> _mockContext;
    private readonly Mock<DbSet<DispatchOrder>> _mockDbSet;
    private readonly CreateDispatchCommandHandler _handler;

    public CreateDispatchCommandHandlerTests()
    {
        _mockDbSet = new Mock<DbSet<DispatchOrder>>();
        _mockContext = new Mock<IFleetDbContext>();
        _mockContext.Setup(c => c.DispatchOrders).Returns(_mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _handler = new CreateDispatchCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_DispatchOrder_And_Return_Id()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var driverId = Guid.NewGuid();

        var command = new CreateDispatchCommand
        {
            TenantId = tenantId,
            FleetCompanyId = companyId,
            VehicleId = vehicleId,
            DriverId = driverId,
            CargoReference = "CARGO-2026-001",
            CustomsCaseReference = "CASE-2026-001"
        };

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);

        // Verify Add and SaveChanges were called
        _mockDbSet.Verify(d => d.Add(It.Is<DispatchOrder>(o =>
            o.TenantId == tenantId &&
            o.VehicleId == vehicleId &&
            o.DriverId == driverId &&
            o.CargoReference == "CARGO-2026-001" &&
            o.Status == "Draft"
        )), Times.Once);

        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Generate_DispatchReference_With_DSP_Prefix()
    {
        // Arrange
        var command = new CreateDispatchCommand
        {
            TenantId = Guid.NewGuid(),
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        DispatchOrder? capturedOrder = null;
        _mockDbSet
            .Setup(d => d.Add(It.IsAny<DispatchOrder>()))
            .Callback<DispatchOrder>(o => capturedOrder = o);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - DispatchReference starts with DSP-
        Assert.NotNull(capturedOrder);
        Assert.StartsWith("DSP-", capturedOrder!.DispatchReference);
    }

    [Fact]
    public async Task Handle_Should_Set_Default_Status_And_Priority()
    {
        // Arrange
        var command = new CreateDispatchCommand
        {
            TenantId = Guid.NewGuid(),
            FleetCompanyId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        DispatchOrder? capturedOrder = null;
        _mockDbSet
            .Setup(d => d.Add(It.IsAny<DispatchOrder>()))
            .Callback<DispatchOrder>(o => capturedOrder = o);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedOrder);
        Assert.Equal("Draft", capturedOrder!.Status);
        Assert.Equal("Normal", capturedOrder.Priority);
    }
}

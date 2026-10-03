using System;
using AisVessel.Domain.Entities;
using AisVessel.Domain.Enums;

namespace AisVessel.Tests.Domain.Entities;

public class PortCallTests
{
    [Fact]
    public void NewPortCall_ShouldDefaultToPlannedStatus()
    {
        // Arrange & Act
        var portCall = new PortCall();

        // Assert
        Assert.Equal("Planned", portCall.Status);
        Assert.Equal(string.Empty, portCall.PortCallReference);
        Assert.Equal(string.Empty, portCall.Purpose);
        Assert.Empty(portCall.Version);
        Assert.Null(portCall.VoyageId);
        Assert.Null(portCall.PlannedArrivalAtUtc);
        Assert.Null(portCall.ActualBerthAtUtc);
        Assert.Null(portCall.ActualDepartureAtUtc);
    }

    [Fact]
    public void NewPortCall_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var portCall = new PortCall();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, portCall.Id);
        Assert.InRange(portCall.CreatedAtUtc, before, after);
        Assert.InRange(portCall.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void AdvanceState_ShouldWriteTheEnumNameAsStringStatus()
    {
        // Arrange: statuses are persisted as the enum names
        var portCall = new PortCall();

        // Act
        portCall.AdvanceState(PortCallStatus.AwaitingBerth);

        // Assert
        Assert.Equal("AwaitingBerth", portCall.Status);
    }

    [Fact]
    public void AdvanceState_ShouldRefreshUpdatedAtUtc()
    {
        // Arrange
        var portCall = new PortCall { UpdatedAtUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc) };

        // Act
        portCall.AdvanceState(PortCallStatus.BerthAllocated);

        // Assert
        Assert.True(portCall.UpdatedAtUtc > new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        Assert.Equal(DateTimeKind.Utc, portCall.UpdatedAtUtc.Kind);
    }

    [Fact]
    public void AdvanceState_CanWalkTheFullBerthLifecycle()
    {
        // Arrange
        var portCall = new PortCall();

        // Act & Assert: each hop stores the matching enum name
        portCall.AdvanceState(PortCallStatus.Expected);
        Assert.Equal("Expected", portCall.Status);

        portCall.AdvanceState(PortCallStatus.ArrivedAnchorage);
        Assert.Equal("ArrivedAnchorage", portCall.Status);

        portCall.AdvanceState(PortCallStatus.AwaitingBerth);
        Assert.Equal("AwaitingBerth", portCall.Status);

        portCall.AdvanceState(PortCallStatus.BerthAllocated);
        Assert.Equal("BerthAllocated", portCall.Status);

        portCall.AdvanceState(PortCallStatus.Berthed);
        Assert.Equal("Berthed", portCall.Status);

        portCall.AdvanceState(PortCallStatus.Departed);
        Assert.Equal("Departed", portCall.Status);
    }
}

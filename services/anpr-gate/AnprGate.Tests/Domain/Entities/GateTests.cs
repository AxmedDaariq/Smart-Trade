using System;
using AnprGate.Domain.Entities;

namespace AnprGate.Tests.Domain.Entities;

public class GateTests
{
    [Fact]
    public void NewGate_ShouldDefaultToActiveNormalOperatingMode()
    {
        // Arrange & Act
        var gate = new Gate();

        // Assert
        Assert.Equal("Active", gate.Status);
        Assert.Equal("Normal", gate.OperatingMode);
        Assert.Equal(string.Empty, gate.GateReference);
        Assert.Equal(string.Empty, gate.Name);
        Assert.Equal(string.Empty, gate.GateType);
        Assert.Null(gate.Location);
        Assert.Empty(gate.Version);
        Assert.Empty(gate.Lanes);
        Assert.Empty(gate.CameraAssignments);
    }

    [Fact]
    public void NewGate_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var gate = new Gate();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, gate.Id);
        Assert.InRange(gate.CreatedAtUtc, before, after);
        Assert.InRange(gate.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewGates_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new Gate();
        var second = new Gate();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void GateProperties_ShouldBeSettable()
    {
        // Arrange
        var gate = new Gate();

        // Act
        gate.GateReference = "GATE-01";
        gate.Name = "Port Entry North";
        gate.GateType = "PortEntry";
        gate.Status = "Maintenance";
        gate.OperatingMode = "ManualOnly";

        // Assert
        Assert.Equal("GATE-01", gate.GateReference);
        Assert.Equal("Port Entry North", gate.Name);
        Assert.Equal("PortEntry", gate.GateType);
        Assert.Equal("Maintenance", gate.Status);
        Assert.Equal("ManualOnly", gate.OperatingMode);
    }
}
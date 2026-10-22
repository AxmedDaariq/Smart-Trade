using System;
using AnprGate.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AnprGate.UnitTests.Domain;

public class GateTests
{
    [Fact]
    public void NewGate_ShouldDefaultToActiveNormalOperatingMode()
    {
        // Arrange & Act
        var gate = new Gate();

        // Assert
        gate.Status.Should().Be("Active");
        gate.OperatingMode.Should().Be("Normal");
        gate.GateReference.Should().BeEmpty();
        gate.Name.Should().BeEmpty();
        gate.GateType.Should().BeEmpty();
        gate.Location.Should().BeNull();
        gate.Version.Should().BeEmpty();
        gate.Lanes.Should().BeEmpty();
        gate.CameraAssignments.Should().BeEmpty();
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
        gate.Id.Should().NotBeEmpty();
        gate.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        gate.UpdatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NewGates_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new Gate();
        var second = new Gate();

        // Assert
        first.Id.Should().NotBe(second.Id);
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
        gate.GateReference.Should().Be("GATE-01");
        gate.Name.Should().Be("Port Entry North");
        gate.GateType.Should().Be("PortEntry");
        gate.Status.Should().Be("Maintenance");
        gate.OperatingMode.Should().Be("ManualOnly");
    }
}

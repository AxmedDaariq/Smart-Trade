using System;
using AisVessel.Domain.Entities;

namespace AisVessel.Tests.Domain.Entities;

public class VesselTests
{
    [Fact]
    public void NewVessel_ShouldDefaultToUnknownStatus()
    {
        // Arrange & Act
        var vessel = new Vessel();

        // Assert
        Assert.Equal("Unknown", vessel.Status);
        Assert.Equal(string.Empty, vessel.VesselReference);
        Assert.Equal(string.Empty, vessel.VesselName);
        Assert.Equal(string.Empty, vessel.VesselType);
        Assert.Null(vessel.ImoNumber);
        Assert.Null(vessel.MmsiNumber);
        Assert.Null(vessel.CallSign);
        Assert.Null(vessel.BuildYear);
        Assert.Null(vessel.GrossTonnage);
        Assert.Null(vessel.UpdatedBy);
        Assert.Empty(vessel.Version);
        Assert.Empty(vessel.Identifiers);
        Assert.Empty(vessel.Characteristics);
    }

    [Fact]
    public void NewVessel_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var vessel = new Vessel();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, vessel.Id);
        Assert.InRange(vessel.CreatedAtUtc, before, after);
        Assert.InRange(vessel.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewVessels_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new Vessel();
        var second = new Vessel();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void VesselProperties_ShouldBeSettable()
    {
        // Arrange
        var vessel = new Vessel();

        // Act
        vessel.VesselName = "Maersk Sentinel";
        vessel.VesselType = "ContainerShip";
        vessel.ImoNumber = "IMO9450123";
        vessel.MmsiNumber = "219883000";
        vessel.Status = "Approaching";
        vessel.BuildYear = 2018;

        // Assert
        Assert.Equal("Maersk Sentinel", vessel.VesselName);
        Assert.Equal("ContainerShip", vessel.VesselType);
        Assert.Equal("IMO9450123", vessel.ImoNumber);
        Assert.Equal("219883000", vessel.MmsiNumber);
        Assert.Equal("Approaching", vessel.Status);
        Assert.Equal(2018, vessel.BuildYear);
    }
}
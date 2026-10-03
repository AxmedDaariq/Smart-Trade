using System;
using System.Linq;
using AisVessel.Domain.Enums;

namespace AisVessel.Tests.Domain.Enums;

public class EnumsTests
{
    [Fact]
    public void PortCallStatus_ShouldContainTheBerthAllocationStates()
    {
        // Assert: the allocation workflow depends on these exact states
        Assert.Equal(0, (int)PortCallStatus.Planned);
        Assert.Equal(PortCallStatus.AwaitingBerth, Enum.Parse<PortCallStatus>("AwaitingBerth"));
        Assert.Equal(PortCallStatus.BerthAllocated, Enum.Parse<PortCallStatus>("BerthAllocated"));
        Assert.Equal(PortCallStatus.Berthed, Enum.Parse<PortCallStatus>("Berthed"));
        Assert.True(Enum.IsDefined(PortCallStatus.AwaitingBerth));
        Assert.False(Enum.IsDefined(typeof(PortCallStatus), 99));
    }

    [Fact]
    public void PortCallStatus_ShouldListTwelveStates()
    {
        Assert.Equal(12, Enum.GetValues<PortCallStatus>().Length);
    }

    [Fact]
    public void PortCallStatus_Names_ShouldMatchWhatIsPersistedAsString()
    {
        // The PortCall entity stores Status as the enum name (e.g. "AwaitingBerth")
        Assert.Equal("AwaitingBerth", nameof(PortCallStatus.AwaitingBerth));
        Assert.Equal("BerthAllocated", nameof(PortCallStatus.BerthAllocated));
    }

    [Theory]
    [InlineData(BerthStatus.Available)]
    [InlineData(BerthStatus.Reserved)]
    [InlineData(BerthStatus.Occupied)]
    [InlineData(BerthStatus.Maintenance)]
    [InlineData(BerthStatus.Closed)]
    [InlineData(BerthStatus.Unavailable)]
    public void BerthStatus_ShouldContainKnownStates(BerthStatus status)
    {
        Assert.True(Enum.IsDefined(status));
    }

    [Fact]
    public void BerthStatus_Available_ShouldBeTheDefaultZeroValue()
    {
        Assert.Equal(0, (int)BerthStatus.Available);
    }

    [Theory]
    [InlineData(VesselType.ContainerShip)]
    [InlineData(VesselType.BulkCarrier)]
    [InlineData(VesselType.Tanker)]
    [InlineData(VesselType.GeneralCargo)]
    [InlineData(VesselType.RoRo)]
    [InlineData(VesselType.Other)]
    public void VesselType_ShouldContainKnownVesselKinds(VesselType vesselType)
    {
        Assert.True(Enum.IsDefined(vesselType));
    }

    [Fact]
    public void VesselStatus_ShouldCoverTheVesselLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)VesselStatus.Unknown);
        Assert.Equal(VesselStatus.Approaching, Enum.Parse<VesselStatus>("Approaching"));
        Assert.Equal(VesselStatus.Anchored, Enum.Parse<VesselStatus>("Anchored"));
        Assert.Equal(VesselStatus.Berthed, Enum.Parse<VesselStatus>("Berthed"));
        Assert.Equal(VesselStatus.AtSea, Enum.Parse<VesselStatus>("AtSea"));
    }

    [Theory]
    [InlineData(PortZoneType.PortBoundary)]
    [InlineData(PortZoneType.Anchorage)]
    [InlineData(PortZoneType.Terminal)]
    [InlineData(PortZoneType.RestrictedArea)]
    [InlineData(PortZoneType.SafetyZone)]
    public void PortZoneType_ShouldContainKnownZoneKinds(PortZoneType zoneType)
    {
        Assert.True(Enum.IsDefined(zoneType));
    }

    [Fact]
    public void AllEnums_ShouldHaveNoDuplicateNames()
    {
        // Assert: duplicate names would break string-based persistence/serialization
        AssertNoDuplicateNames<PortCallStatus>();
        AssertNoDuplicateNames<BerthStatus>();
        AssertNoDuplicateNames<VesselType>();
        AssertNoDuplicateNames<VesselStatus>();
        AssertNoDuplicateNames<PortZoneType>();
    }

    private static void AssertNoDuplicateNames<T>() where T : struct, Enum
    {
        var names = Enum.GetNames<T>();
        Assert.Equal(names.Length, names.Distinct().Count());
    }
}

using System;
using System.Linq;
using GpsTelematics.Domain.Enums;

namespace GpsTelematics.Tests.Domain.Enums;

public class EnumsTests
{
    [Theory]
    [InlineData(DeviceStatus.Provisioning)]
    [InlineData(DeviceStatus.Active)]
    [InlineData(DeviceStatus.Online)]
    [InlineData(DeviceStatus.Offline)]
    [InlineData(DeviceStatus.Suspended)]
    [InlineData(DeviceStatus.Retired)]
    public void DeviceStatus_ShouldContainKnownStates(DeviceStatus status)
    {
        Assert.True(Enum.IsDefined(status));
    }

    [Fact]
    public void DeviceStatus_Provisioning_ShouldBeTheDefaultZeroValue()
    {
        Assert.Equal(0, (int)DeviceStatus.Provisioning);
    }

    [Theory]
    [InlineData(DeviceType.GPS)]
    [InlineData(DeviceType.OBD)]
    [InlineData(DeviceType.CANBus)]
    [InlineData(DeviceType.Telematics)]
    [InlineData(DeviceType.Tracker)]
    [InlineData(DeviceType.Other)]
    public void DeviceType_ShouldContainKnownDeviceKinds(DeviceType deviceType)
    {
        Assert.True(Enum.IsDefined(deviceType));
    }

    [Fact]
    public void AlertSeverity_ShouldBeOrderedFromInfoToCritical()
    {
        // Assert: severity ordering matters for alert routing/thresholds
        Assert.Equal(0, (int)AlertSeverity.Info);
        Assert.Equal(1, (int)AlertSeverity.Low);
        Assert.Equal(2, (int)AlertSeverity.Medium);
        Assert.Equal(3, (int)AlertSeverity.High);
        Assert.Equal(4, (int)AlertSeverity.Critical);
    }

    [Fact]
    public void TrackingSessionStatus_ShouldCoverTheSessionLifecycle()
    {
        // Assert
        Assert.Equal(0, (int)TrackingSessionStatus.Created);
        Assert.Equal(TrackingSessionStatus.Active, Enum.Parse<TrackingSessionStatus>("Active"));
        Assert.Equal(TrackingSessionStatus.Paused, Enum.Parse<TrackingSessionStatus>("Paused"));
        Assert.Equal(TrackingSessionStatus.Completed, Enum.Parse<TrackingSessionStatus>("Completed"));
        Assert.Equal(TrackingSessionStatus.Cancelled, Enum.Parse<TrackingSessionStatus>("Cancelled"));
        Assert.False(Enum.IsDefined(typeof(TrackingSessionStatus), 99));
    }

    [Theory]
    [InlineData(GeofenceType.Port)]
    [InlineData(GeofenceType.Terminal)]
    [InlineData(GeofenceType.Border)]
    [InlineData(GeofenceType.DryPort)]
    [InlineData(GeofenceType.RestrictedArea)]
    [InlineData(GeofenceType.Custom)]
    public void GeofenceType_ShouldContainKnownZoneKinds(GeofenceType geofenceType)
    {
        Assert.True(Enum.IsDefined(geofenceType));
    }

    [Fact]
    public void AllEnums_ShouldHaveNoDuplicateNames()
    {
        // Assert: duplicate names would break string-based persistence/serialization
        AssertNoDuplicateNames<DeviceStatus>();
        AssertNoDuplicateNames<DeviceType>();
        AssertNoDuplicateNames<AlertSeverity>();
        AssertNoDuplicateNames<TrackingSessionStatus>();
        AssertNoDuplicateNames<GeofenceType>();
    }

    private static void AssertNoDuplicateNames<T>() where T : struct, Enum
    {
        var names = Enum.GetNames<T>();
        Assert.Equal(names.Length, names.Distinct().Count());
    }
}

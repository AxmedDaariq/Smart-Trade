using System;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Tests.Domain.Entities;

public class GpsDeviceTests
{
    [Fact]
    public void NewGpsDevice_ShouldDefaultToPendingStatus()
    {
        // Arrange & Act
        var device = new GpsDevice();

        // Assert
        Assert.Equal("Pending", device.Status);
        Assert.Equal(string.Empty, device.DeviceReference);
        Assert.Equal(string.Empty, device.DeviceSerialNumber);
        Assert.Equal(string.Empty, device.DeviceType);
        Assert.Equal(string.Empty, device.Protocol);
        Assert.Null(device.Imei);
        Assert.Null(device.LastConnectedAtUtc);
        Assert.Null(device.LastSeenAtUtc);
        Assert.Null(device.UpdatedBy);
        Assert.Empty(device.Version);
    }

    [Fact]
    public void NewGpsDevice_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var device = new GpsDevice();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, device.Id);
        Assert.InRange(device.CreatedAtUtc, before, after);
        Assert.InRange(device.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewGpsDevices_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new GpsDevice();
        var second = new GpsDevice();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void GpsDeviceProperties_ShouldBeSettable()
    {
        // Arrange
        var device = new GpsDevice();
        var lastSeen = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        device.DeviceReference = "TRK-001";
        device.DeviceSerialNumber = "SN-9911";
        device.Status = "Active";
        device.LastSeenAtUtc = lastSeen;
        device.UpdatedBy = Guid.NewGuid();

        // Assert
        Assert.Equal("TRK-001", device.DeviceReference);
        Assert.Equal("SN-9911", device.DeviceSerialNumber);
        Assert.Equal("Active", device.Status);
        Assert.Equal(lastSeen, device.LastSeenAtUtc);
        Assert.NotNull(device.UpdatedBy);
    }
}
using System;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Tests.Domain.Entities;

public class GpsPositionTests
{
    [Fact]
    public void NewPosition_ShouldDefaultToAcceptedValidationStatus()
    {
        // Arrange & Act
        var position = new GpsPosition();

        // Assert
        Assert.Equal("Accepted", position.ValidationStatus);
        Assert.Equal(string.Empty, position.VehicleReference);
        Assert.Equal(string.Empty, position.Source);
        Assert.Null(position.SpeedKph);
        Assert.Null(position.HeadingDegrees);
        Assert.Null(position.TrackingSessionId);
        Assert.Null(position.BatteryPercent);
        Assert.Null(position.IsMockLocation);
        Assert.Null(position.AltitudeMeters);
    }

    [Fact]
    public void NewPosition_ShouldHaveUniqueIdAndStampedTimestamps()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var position = new GpsPosition();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, position.Id);
        Assert.InRange(position.ReceivedAtUtc, before, after);
        Assert.InRange(position.CreatedAtUtc, before, after);
    }

    [Fact]
    public void NewPositions_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new GpsPosition();
        var second = new GpsPosition();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void PositionProperties_ShouldBeSettable()
    {
        // Arrange
        var position = new GpsPosition();

        // Act
        position.ValidationStatus = "Quarantined";
        position.SequenceNumber = 9001;
        position.Latitude = 11.5919;
        position.Longitude = 43.1456;
        position.SpeedKph = 65.4;
        position.HeadingDegrees = 270;
        position.TimestampUtc = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

        // Assert
        Assert.Equal("Quarantined", position.ValidationStatus);
        Assert.Equal(9001, position.SequenceNumber);
        Assert.Equal(11.5919, position.Latitude);
        Assert.Equal(43.1456, position.Longitude);
        Assert.Equal(65.4, position.SpeedKph);
        Assert.Equal(270, position.HeadingDegrees);
        Assert.Equal(DateTimeKind.Utc, position.TimestampUtc.Kind);
    }
}

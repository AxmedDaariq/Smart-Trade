using System;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Tests.Domain.Entities;

public class TrackingSessionTests
{
    [Fact]
    public void NewTrackingSession_ShouldDefaultToStartingStatus()
    {
        // Arrange & Act
        var session = new TrackingSession();

        // Assert
        Assert.Equal("Starting", session.Status);
        Assert.Equal(string.Empty, session.VehicleReference);
        Assert.Null(session.DriverReference);
        Assert.Null(session.DispatchReference);
        Assert.Null(session.TripReference);
        Assert.Null(session.EndedAtUtc);
        Assert.Null(session.StartPosition);
        Assert.Null(session.EndPosition);
        Assert.Empty(session.Version);
    }

    [Fact]
    public void NewTrackingSession_ShouldHaveUniqueIdAndStampedStartAndCreationTimes()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var session = new TrackingSession();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.InRange(session.StartedAtUtc, before, after);
        Assert.InRange(session.CreatedAtUtc, before, after);
        Assert.InRange(session.UpdatedAtUtc, before, after);
    }

    [Fact]
    public void NewTrackingSessions_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var first = new TrackingSession();
        var second = new TrackingSession();

        // Assert
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void TrackingSessionProperties_ShouldBeSettable()
    {
        // Arrange
        var session = new TrackingSession();
        var endedAt = new DateTime(2026, 3, 1, 14, 30, 0, DateTimeKind.Utc);

        // Act
        session.Status = "Completed";
        session.VehicleReference = "VEH-42";
        session.DriverReference = "DRV-7";
        session.EndedAtUtc = endedAt;

        // Assert
        Assert.Equal("Completed", session.Status);
        Assert.Equal("VEH-42", session.VehicleReference);
        Assert.Equal("DRV-7", session.DriverReference);
        Assert.Equal(endedAt, session.EndedAtUtc);
        Assert.Equal(DateTimeKind.Utc, session.EndedAtUtc!.Value.Kind);
    }
}
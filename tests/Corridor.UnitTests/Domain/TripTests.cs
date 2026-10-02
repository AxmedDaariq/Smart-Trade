using Corridor.Domain.Entities;
using Corridor.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Corridor.UnitTests.Domain;

public class TripTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    // ── Constructor / defaults ─────────────────────────────────────────────────

    [Fact]
    public void Constructor_DefaultValues_SetsStatusToPlanned()
    {
        var trip = BuildTrip();
        trip.Status.Should().Be("Planned");
    }

    [Fact]
    public void Constructor_DefaultValues_SetsDeviationStatusToNormal()
    {
        var trip = BuildTrip();
        trip.DeviationStatus.Should().Be("Normal");
    }

    [Fact]
    public void Constructor_NewTrip_HasEmptyCollections()
    {
        var trip = BuildTrip();
        trip.Milestones.Should().BeEmpty();
        trip.Delays.Should().BeEmpty();
        trip.Deviations.Should().BeEmpty();
        trip.Incidents.Should().BeEmpty();
        trip.Alerts.Should().BeEmpty();
        trip.Holds.Should().BeEmpty();
        trip.CheckpointEvents.Should().BeEmpty();
        trip.StatusHistory.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_AutoGeneratesNonEmptyId()
    {
        var trip = BuildTrip();
        trip.Id.Should().NotBe(Guid.Empty);
    }

    // ── Trip references ────────────────────────────────────────────────────────

    [Fact]
    public void Trip_CanBeLinkedToCorridorAndRoute()
    {
        var corridorId = Guid.NewGuid();
        var routeId    = Guid.NewGuid();

        var trip = BuildTrip();
        trip.CorridorId = corridorId;
        trip.RouteId    = routeId;

        trip.CorridorId.Should().Be(corridorId);
        trip.RouteId.Should().Be(routeId);
    }

    // ── Planned dates ──────────────────────────────────────────────────────────

    [Fact]
    public void Trip_PlannedDates_CanBeSet()
    {
        var start   = DateTime.UtcNow.AddHours(2);
        var arrival = DateTime.UtcNow.AddHours(14);

        var trip = BuildTrip();
        trip.PlannedStartAtUtc    = start;
        trip.PlannedArrivalAtUtc  = arrival;

        trip.PlannedStartAtUtc.Should().BeCloseTo(start,   TimeSpan.FromSeconds(1));
        trip.PlannedArrivalAtUtc.Should().BeCloseTo(arrival, TimeSpan.FromSeconds(1));
    }

    // ── DeviationStatus ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("Normal")]
    [InlineData("Warning")]
    [InlineData("Deviated")]
    [InlineData("Unknown")]
    public void Trip_DeviationStatus_CanBeSetToValidValues(string status)
    {
        var trip = BuildTrip();
        trip.DeviationStatus = status;
        trip.DeviationStatus.Should().Be(status);
    }

    // ── DelayMinutes ───────────────────────────────────────────────────────────

    [Fact]
    public void Trip_DelayMinutes_DefaultIsNull()
    {
        var trip = BuildTrip();
        trip.DelayMinutes.Should().BeNull();
    }

    [Fact]
    public void Trip_DelayMinutes_CanBeAssigned()
    {
        var trip = BuildTrip();
        trip.DelayMinutes = 45.5;
        trip.DelayMinutes.Should().Be(45.5);
    }

    // ── GPS Position ───────────────────────────────────────────────────────────

    [Fact]
    public void Trip_GpsCoordinates_DefaultIsNull()
    {
        var trip = BuildTrip();
        trip.CurrentLatitude.Should().BeNull();
        trip.CurrentLongitude.Should().BeNull();
        trip.CurrentPosition.Should().BeNull();
    }

    [Fact]
    public void Trip_GpsCoordinates_CanBeUpdated()
    {
        var trip = BuildTrip();
        trip.CurrentLatitude     = 2.0469;   // Mogadishu
        trip.CurrentLongitude    = 45.3182;
        trip.LastPositionAtUtc   = DateTime.UtcNow;

        trip.CurrentLatitude.Should().BeApproximately(2.0469,  0.0001);
        trip.CurrentLongitude.Should().BeApproximately(45.3182, 0.0001);
        trip.LastPositionAtUtc.Should().NotBeNull();
    }

    // ── Milestone collection ───────────────────────────────────────────────────

    [Fact]
    public void Trip_AddingMilestone_IncreasesCollection()
    {
        var trip      = BuildTrip();
        var milestone = new TripMilestone
        {
            TripId          = trip.Id,
            TenantId        = TenantId,
            MilestoneType   = MilestoneType.PortDeparture.ToString(),
            SequenceNumber  = 1,
            Status          = "Pending"
        };

        trip.Milestones.Add(milestone);

        trip.Milestones.Should().HaveCount(1);
        trip.Milestones.First().MilestoneType.Should().Be("PortDeparture");
    }

    // ── Delay collection ──────────────────────────────────────────────────────

    [Fact]
    public void Trip_AddingDelay_IncreasesCollection()
    {
        var trip  = BuildTrip();
        var delay = new TripDelay
        {
            TripId         = trip.Id,
            TenantId       = TenantId,
            DelayType      = "Border",
            Reason         = "Document check",
            StartedAtUtc   = DateTime.UtcNow,
            Status         = "Open"
        };

        trip.Delays.Add(delay);

        trip.Delays.Should().HaveCount(1);
        trip.Delays.First().DelayType.Should().Be("Border");
        trip.Delays.First().Status.Should().Be("Open");
    }

    // ── ETA ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Trip_Eta_DefaultIsNull()
    {
        var trip = BuildTrip();
        trip.CurrentEtaAtUtc.Should().BeNull();
    }

    [Fact]
    public void Trip_Eta_CanBeSet()
    {
        var trip = BuildTrip();
        var eta  = DateTime.UtcNow.AddHours(6);
        trip.CurrentEtaAtUtc = eta;
        trip.CurrentEtaAtUtc.Should().BeCloseTo(eta, TimeSpan.FromSeconds(1));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Trip BuildTrip() => new()
    {
        TenantId           = TenantId,
        TripReference      = "TRIP-2026-001",
        DispatchReference  = "DISP-001",
        VehicleReference   = "VEH-001",
        DriverReference    = "DRV-001",
        Origin             = "Mogadishu Port",
        Destination        = "Nairobi ICD",
        PlannedStartAtUtc  = DateTime.UtcNow.AddHours(2)
    };
}

using Corridor.Domain.Entities;
using Corridor.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Corridor.UnitTests.Domain;

public class RouteDeviationTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid TripId   = Guid.NewGuid();

    // ── Default state ─────────────────────────────────────────────────────────

    [Fact]
    public void RouteDeviation_DefaultStatus_IsOpen()
    {
        var deviation = BuildDeviation();
        deviation.Status.Should().Be("Open");
    }

    [Fact]
    public void RouteDeviation_DefaultSeverity_IsWarning()
    {
        var deviation = BuildDeviation();
        deviation.Severity.Should().Be("Warning");
    }

    [Fact]
    public void RouteDeviation_DetectedAtUtc_IsSetOnCreation()
    {
        var before    = DateTime.UtcNow.AddSeconds(-1);
        var deviation = BuildDeviation();
        deviation.DetectedAtUtc.Should().BeAfter(before);
    }

    [Fact]
    public void RouteDeviation_ResolvedAtUtc_DefaultIsNull()
    {
        var deviation = BuildDeviation();
        deviation.ResolvedAtUtc.Should().BeNull();
    }

    // ── Distance ──────────────────────────────────────────────────────────────

    [Fact]
    public void RouteDeviation_DistanceFromRoute_CanBeSet()
    {
        var deviation = BuildDeviation();
        deviation.DistanceFromPlannedRouteMeters = 1250.75;
        deviation.DistanceFromPlannedRouteMeters.Should().BeApproximately(1250.75, 0.01);
    }

    [Fact]
    public void RouteDeviation_DeviationDurationMinutes_DefaultIsNull()
    {
        var deviation = BuildDeviation();
        deviation.DeviationDurationMinutes.Should().BeNull();
    }

    // ── Status transitions ────────────────────────────────────────────────────

    [Theory]
    [InlineData("Open")]
    [InlineData("Acknowledged")]
    [InlineData("Resolved")]
    [InlineData("Dismissed")]
    public void RouteDeviation_Status_AcceptsAllValidValues(string status)
    {
        var deviation = BuildDeviation();
        deviation.Status = status;
        deviation.Status.Should().Be(status);
    }

    // ── Severity levels ───────────────────────────────────────────────────────

    [Theory]
    [InlineData("Warning")]
    [InlineData("High")]
    [InlineData("Critical")]
    public void RouteDeviation_Severity_AcceptsValidValues(string severity)
    {
        var deviation = BuildDeviation();
        deviation.Severity = severity;
        deviation.Severity.Should().Be(severity);
    }

    // ── Reason ────────────────────────────────────────────────────────────────

    [Fact]
    public void RouteDeviation_Reason_DefaultIsNull()
    {
        var deviation = BuildDeviation();
        deviation.Reason.Should().BeNull();
    }

    [Fact]
    public void RouteDeviation_Reason_CanBeSet()
    {
        var deviation = BuildDeviation();
        deviation.Reason = "Unauthorised road used";
        deviation.Reason.Should().Be("Unauthorised road used");
    }

    // ── Resolution ────────────────────────────────────────────────────────────

    [Fact]
    public void RouteDeviation_WhenResolved_ResolvedAtUtcIsSet()
    {
        var deviation = BuildDeviation();
        var resolvedAt = DateTime.UtcNow;

        deviation.Status       = "Resolved";
        deviation.ResolvedAtUtc = resolvedAt;

        deviation.Status.Should().Be("Resolved");
        deviation.ResolvedAtUtc.Should().BeCloseTo(resolvedAt, TimeSpan.FromSeconds(1));
    }

    // ── TripExceptionType enum ────────────────────────────────────────────────

    [Fact]
    public void TripExceptionType_RouteDeviation_Exists()
    {
        var values = Enum.GetNames(typeof(TripExceptionType));
        values.Should().Contain("RouteDeviation");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static RouteDeviation BuildDeviation() => new()
    {
        TenantId                       = TenantId,
        TripId                         = TripId,
        DistanceFromPlannedRouteMeters = 500.0,
        DetectionSource                = "GPS",
        Severity                       = "Warning",
        Status                         = "Open"
    };
}

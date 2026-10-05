using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditStatusHistory"/> (append-only status transitions).
/// </summary>
public class AuditStatusHistoryTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var history = new AuditStatusHistory();
        var after  = DateTime.UtcNow.AddSeconds(1);

        history.Id.Should().NotBe(Guid.Empty);
        history.TenantId.Should().BeNull();
        history.AuditEventId.Should().Be(Guid.Empty);
        history.PreviousStatus.Should().BeNull();
        history.NewStatus.Should().BeEmpty();
        history.Reason.Should().BeNull();
        history.OccurredAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        history.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var history = new AuditStatusHistory();
        history.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var occurred = new DateTime(2026, 4, 10, 12, 0, 0, DateTimeKind.Utc);

        var history = new AuditStatusHistory
        {
            TenantId       = Guid.NewGuid(),
            AuditEventId   = Guid.NewGuid(),
            PreviousStatus = "Open",
            NewStatus      = "Resolved",
            Reason         = "Investigated and cleared",
            OccurredAtUtc  = occurred
        };

        history.PreviousStatus.Should().Be("Open");
        history.NewStatus.Should().Be("Resolved");
        history.Reason.Should().Be("Investigated and cleared");
        history.OccurredAtUtc.Should().Be(occurred);
    }
}

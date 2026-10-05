using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditChange"/> (before/after snapshot ledger entries).
/// </summary>
public class AuditChangeTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var change = new AuditChange();
        var after  = DateTime.UtcNow.AddSeconds(1);

        change.Id.Should().NotBe(Guid.Empty);
        change.TenantId.Should().BeNull();
        change.AuditEventId.Should().Be(Guid.Empty);
        change.ResourceType.Should().BeEmpty();
        change.ResourceId.Should().Be(Guid.Empty);
        change.ChangeType.Should().BeEmpty();
        change.BeforeSnapshotJson.Should().BeNull();
        change.AfterSnapshotJson.Should().BeNull();
        change.ChangedFieldsJson.Should().BeNull();
        change.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var change = new AuditChange();
        change.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var eventId    = Guid.NewGuid();
        var resourceId = Guid.NewGuid();

        var change = new AuditChange
        {
            TenantId           = Guid.NewGuid(),
            AuditEventId       = eventId,
            ResourceType       = "Payment",
            ResourceId         = resourceId,
            ChangeType         = "Updated",
            BeforeSnapshotJson = "{\"status\":\"Pending\"}",
            AfterSnapshotJson  = "{\"status\":\"Completed\"}",
            ChangedFieldsJson  = "[\"status\"]"
        };

        change.AuditEventId.Should().Be(eventId);
        change.ResourceType.Should().Be("Payment");
        change.ResourceId.Should().Be(resourceId);
        change.ChangeType.Should().Be("Updated");
        change.BeforeSnapshotJson.Should().Be("{\"status\":\"Pending\"}");
        change.AfterSnapshotJson.Should().Be("{\"status\":\"Completed\"}");
        change.ChangedFieldsJson.Should().Be("[\"status\"]");
    }
}

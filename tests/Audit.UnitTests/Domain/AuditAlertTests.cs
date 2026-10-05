using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditAlert"/>.
/// </summary>
public class AuditAlertTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var alert  = new AuditAlert();
        var after  = DateTime.UtcNow.AddSeconds(1);

        alert.Id.Should().NotBe(Guid.Empty);
        alert.TenantId.Should().BeNull();
        alert.AlertReference.Should().BeEmpty();
        alert.AlertType.Should().BeEmpty();
        alert.Severity.Should().BeEmpty();
        alert.SourceAuditEventId.Should().BeNull();
        alert.Description.Should().BeEmpty();
        alert.Status.Should().Be("Open");
        alert.AcknowledgedBy.Should().BeNull();
        alert.AcknowledgedAtUtc.Should().BeNull();
        alert.ResolvedBy.Should().BeNull();
        alert.ResolvedAtUtc.Should().BeNull();
        alert.DetectedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        alert.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var sourceEventId = Guid.NewGuid();
        var acknowledgedBy = Guid.NewGuid();

        var alert = new AuditAlert
        {
            TenantId           = Guid.NewGuid(),
            AlertReference     = "ALT-2026-0001",
            AlertType          = "AuditTampering",
            Severity           = "Critical",
            SourceAuditEventId = sourceEventId,
            Description        = "Hash chain discontinuity detected",
            Status             = "Acknowledged",
            AcknowledgedBy     = acknowledgedBy
        };

        alert.AlertReference.Should().Be("ALT-2026-0001");
        alert.AlertType.Should().Be("AuditTampering");
        alert.Severity.Should().Be("Critical");
        alert.SourceAuditEventId.Should().Be(sourceEventId);
        alert.Description.Should().Be("Hash chain discontinuity detected");
        alert.Status.Should().Be("Acknowledged");
        alert.AcknowledgedBy.Should().Be(acknowledgedBy);
    }
}

using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditLegalHold"/>.
/// </summary>
public class AuditLegalHoldTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var hold   = new AuditLegalHold();
        var after  = DateTime.UtcNow.AddSeconds(1);

        hold.Id.Should().NotBe(Guid.Empty);
        hold.TenantId.Should().BeNull();
        hold.Reference.Should().BeEmpty();
        hold.Reason.Should().BeEmpty();
        hold.CreatedBy.Should().Be(Guid.Empty);
        hold.ApprovedBy.Should().BeNull();
        hold.EndAtUtc.Should().BeNull();
        hold.Status.Should().Be("Active");
        hold.StartAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        hold.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var createdBy = Guid.NewGuid();
        var approvedBy = Guid.NewGuid();
        var endAt = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        var hold = new AuditLegalHold
        {
            TenantId   = Guid.NewGuid(),
            Reference  = "LH-2026-01",
            Reason     = "Ongoing litigation",
            CreatedBy  = createdBy,
            ApprovedBy = approvedBy,
            EndAtUtc   = endAt,
            Status     = "Released"
        };

        hold.Reference.Should().Be("LH-2026-01");
        hold.Reason.Should().Be("Ongoing litigation");
        hold.CreatedBy.Should().Be(createdBy);
        hold.ApprovedBy.Should().Be(approvedBy);
        hold.EndAtUtc.Should().Be(endAt);
        hold.Status.Should().Be("Released");
    }
}

using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditApproval"/>.
/// </summary>
public class AuditApprovalTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var approval = new AuditApproval();
        var after  = DateTime.UtcNow.AddSeconds(1);

        approval.Id.Should().NotBe(Guid.Empty);
        approval.TenantId.Should().BeNull();
        approval.AuditEventId.Should().Be(Guid.Empty);
        approval.ResourceType.Should().BeEmpty();
        approval.ResourceId.Should().Be(Guid.Empty);
        approval.ApprovalType.Should().BeEmpty();
        approval.SubmittedBy.Should().Be(Guid.Empty);
        approval.ReviewedBy.Should().BeNull();
        approval.Decision.Should().BeEmpty();
        approval.Reason.Should().BeNull();

        // SubmittedAtUtc has no initializer; it is supplied by the producer.
        approval.SubmittedAtUtc.Should().Be(default(DateTime));
        approval.ReviewedAtUtc.Should().BeNull();
        approval.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var approval = new AuditApproval();
        approval.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var submittedBy = Guid.NewGuid();
        var reviewedBy  = Guid.NewGuid();
        var submittedAt = new DateTime(2026, 2, 1, 8, 0, 0, DateTimeKind.Utc);
        var reviewedAt  = submittedAt.AddHours(3);

        var approval = new AuditApproval
        {
            TenantId       = Guid.NewGuid(),
            AuditEventId   = Guid.NewGuid(),
            ResourceType   = "LegalHold",
            ApprovalType   = "Compliance",
            SubmittedBy    = submittedBy,
            ReviewedBy     = reviewedBy,
            Decision       = "Approved",
            Reason         = "Reviewed and accepted",
            SubmittedAtUtc = submittedAt,
            ReviewedAtUtc  = reviewedAt
        };

        approval.SubmittedBy.Should().Be(submittedBy);
        approval.ReviewedBy.Should().Be(reviewedBy);
        approval.Decision.Should().Be("Approved");
        approval.Reason.Should().Be("Reviewed and accepted");
        approval.SubmittedAtUtc.Should().Be(submittedAt);
        approval.ReviewedAtUtc.Should().Be(reviewedAt);
    }
}

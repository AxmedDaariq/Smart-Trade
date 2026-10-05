using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditRetentionPolicy"/>.
/// </summary>
public class AuditRetentionPolicyTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var policy = new AuditRetentionPolicy();
        var after  = DateTime.UtcNow.AddSeconds(1);

        policy.Id.Should().NotBe(Guid.Empty);
        policy.TenantId.Should().BeNull();
        policy.Category.Should().BeEmpty();
        policy.MinimumRetentionDays.Should().Be(0);
        policy.ArchiveAfterDays.Should().BeNull();
        policy.DeleteAfterDays.Should().BeNull();
        policy.LegalHoldEnabled.Should().BeFalse();
        policy.Status.Should().Be("Active");
        policy.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        policy.UpdatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var policy = new AuditRetentionPolicy
        {
            TenantId             = Guid.NewGuid(),
            Category             = "Security",
            MinimumRetentionDays = 2555,
            ArchiveAfterDays     = 365,
            DeleteAfterDays      = 2555,
            LegalHoldEnabled     = true,
            Status               = "Inactive"
        };

        policy.Category.Should().Be("Security");
        policy.MinimumRetentionDays.Should().Be(2555);
        policy.ArchiveAfterDays.Should().Be(365);
        policy.DeleteAfterDays.Should().Be(2555);
        policy.LegalHoldEnabled.Should().BeTrue();
        policy.Status.Should().Be("Inactive");
    }
}

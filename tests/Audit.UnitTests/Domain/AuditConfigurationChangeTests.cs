using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditConfigurationChange"/>.
/// </summary>
public class AuditConfigurationChangeTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var change = new AuditConfigurationChange();
        var after  = DateTime.UtcNow.AddSeconds(1);

        change.Id.Should().NotBe(Guid.Empty);
        change.TenantId.Should().BeNull();
        change.AuditEventId.Should().Be(Guid.Empty);
        change.ConfigurationType.Should().BeEmpty();
        change.ConfigurationReference.Should().BeEmpty();
        change.VersionNumber.Should().BeNull();
        change.PreviousVersionNumber.Should().BeNull();
        change.ChangeType.Should().BeEmpty();
        change.LegalBasis.Should().BeNull();
        change.Reason.Should().BeNull();
        change.EffectiveFromUtc.Should().BeNull();
        change.EffectiveToUtc.Should().BeNull();
        change.ChangedBy.Should().Be(Guid.Empty);
        change.ApprovedBy.Should().BeNull();
        change.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var change = new AuditConfigurationChange();
        change.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var changedBy = Guid.NewGuid();
        var effectiveFrom = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        var change = new AuditConfigurationChange
        {
            TenantId              = Guid.NewGuid(),
            AuditEventId          = Guid.NewGuid(),
            ConfigurationType     = "RetentionPolicy",
            ConfigurationReference= "RET-CUSTOMS",
            VersionNumber         = "2",
            PreviousVersionNumber = "1",
            ChangeType            = "Updated",
            LegalBasis            = "Customs Act 2026",
            Reason                = "Extended retention",
            EffectiveFromUtc      = effectiveFrom,
            ChangedBy             = changedBy,
            ApprovedBy            = Guid.NewGuid()
        };

        change.ConfigurationType.Should().Be("RetentionPolicy");
        change.ConfigurationReference.Should().Be("RET-CUSTOMS");
        change.VersionNumber.Should().Be("2");
        change.PreviousVersionNumber.Should().Be("1");
        change.ChangeType.Should().Be("Updated");
        change.LegalBasis.Should().Be("Customs Act 2026");
        change.EffectiveFromUtc.Should().Be(effectiveFrom);
        change.ChangedBy.Should().Be(changedBy);
    }
}

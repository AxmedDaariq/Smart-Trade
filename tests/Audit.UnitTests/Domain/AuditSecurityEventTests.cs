using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditSecurityEvent"/>.
/// </summary>
public class AuditSecurityEventTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var securityEvent = new AuditSecurityEvent();
        var after  = DateTime.UtcNow.AddSeconds(1);

        securityEvent.Id.Should().NotBe(Guid.Empty);
        securityEvent.TenantId.Should().BeNull();
        securityEvent.AuditEventId.Should().Be(Guid.Empty);
        securityEvent.SecurityEventType.Should().BeEmpty();
        securityEvent.RiskLevel.Should().BeEmpty();
        securityEvent.DetectionSource.Should().BeEmpty();
        securityEvent.IpAddress.Should().BeNull();
        securityEvent.DeviceReference.Should().BeNull();
        securityEvent.UserReference.Should().BeNull();
        securityEvent.ResourceReference.Should().BeNull();
        securityEvent.DetectionReason.Should().BeEmpty();
        securityEvent.Status.Should().Be("Open");
        securityEvent.ResolvedAtUtc.Should().BeNull();
        securityEvent.ResolvedBy.Should().BeNull();
        securityEvent.DetectedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        securityEvent.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var securityEvent = new AuditSecurityEvent();
        securityEvent.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var resolvedBy = Guid.NewGuid();

        var securityEvent = new AuditSecurityEvent
        {
            TenantId            = Guid.NewGuid(),
            AuditEventId        = Guid.NewGuid(),
            SecurityEventType   = "PrivilegeEscalation",
            RiskLevel           = "Critical",
            DetectionSource     = "RuleEngine",
            IpAddress           = "10.0.0.5",
            DeviceReference     = "device-1",
            UserReference       = "user-1",
            ResourceReference   = "res-1",
            DetectionReason     = "Unexpected role grant",
            Status              = "Investigating",
            ResolvedBy          = resolvedBy
        };

        securityEvent.SecurityEventType.Should().Be("PrivilegeEscalation");
        securityEvent.RiskLevel.Should().Be("Critical");
        securityEvent.DetectionSource.Should().Be("RuleEngine");
        securityEvent.IpAddress.Should().Be("10.0.0.5");
        securityEvent.DeviceReference.Should().Be("device-1");
        securityEvent.UserReference.Should().Be("user-1");
        securityEvent.ResourceReference.Should().Be("res-1");
        securityEvent.DetectionReason.Should().Be("Unexpected role grant");
        securityEvent.Status.Should().Be("Investigating");
        securityEvent.ResolvedBy.Should().Be(resolvedBy);
    }
}

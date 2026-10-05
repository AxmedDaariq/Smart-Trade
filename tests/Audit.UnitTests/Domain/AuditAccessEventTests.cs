using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditAccessEvent"/>.
/// </summary>
public class AuditAccessEventTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var accessEvent = new AuditAccessEvent();
        var after  = DateTime.UtcNow.AddSeconds(1);

        accessEvent.Id.Should().NotBe(Guid.Empty);
        accessEvent.TenantId.Should().BeNull();
        accessEvent.AuditEventId.Should().Be(Guid.Empty);
        accessEvent.UserReference.Should().BeNull();
        accessEvent.ResourceType.Should().BeEmpty();
        accessEvent.ResourceId.Should().BeNull();
        accessEvent.PermissionRequired.Should().BeNull();
        accessEvent.PermissionResult.Should().BeEmpty();
        accessEvent.AccessMethod.Should().BeEmpty();
        accessEvent.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var accessEvent = new AuditAccessEvent();
        accessEvent.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var resourceId = Guid.NewGuid();

        var accessEvent = new AuditAccessEvent
        {
            TenantId           = Guid.NewGuid(),
            AuditEventId       = Guid.NewGuid(),
            UserReference      = "user-42",
            ResourceType       = "AuditExport",
            ResourceId         = resourceId,
            PermissionRequired = "audit.read",
            PermissionResult   = "Granted",
            AccessMethod       = "API"
        };

        accessEvent.UserReference.Should().Be("user-42");
        accessEvent.ResourceType.Should().Be("AuditExport");
        accessEvent.ResourceId.Should().Be(resourceId);
        accessEvent.PermissionRequired.Should().Be("audit.read");
        accessEvent.PermissionResult.Should().Be("Granted");
        accessEvent.AccessMethod.Should().Be("API");
    }
}

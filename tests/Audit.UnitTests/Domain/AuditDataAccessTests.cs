using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditDataAccess"/>.
/// </summary>
public class AuditDataAccessTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var access = new AuditDataAccess();
        var after  = DateTime.UtcNow.AddSeconds(1);

        access.Id.Should().NotBe(Guid.Empty);
        access.TenantId.Should().BeNull();
        access.AuditEventId.Should().Be(Guid.Empty);
        access.ResourceType.Should().BeEmpty();
        access.ResourceId.Should().BeNull();
        access.AccessType.Should().BeEmpty();
        access.Purpose.Should().BeNull();
        access.Result.Should().BeEmpty();
        access.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var access = new AuditDataAccess();
        access.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var resourceId = Guid.NewGuid();

        var access = new AuditDataAccess
        {
            TenantId     = Guid.NewGuid(),
            AuditEventId = Guid.NewGuid(),
            ResourceType = "Declaration",
            ResourceId   = resourceId,
            AccessType   = "Export",
            Purpose      = "Audit investigation",
            Result       = "Success"
        };

        access.ResourceType.Should().Be("Declaration");
        access.ResourceId.Should().Be(resourceId);
        access.AccessType.Should().Be("Export");
        access.Purpose.Should().Be("Audit investigation");
        access.Result.Should().Be("Success");
    }
}

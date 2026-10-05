using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditResourceReference"/>.
/// </summary>
public class AuditResourceReferenceTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var reference = new AuditResourceReference();
        var after = DateTime.UtcNow.AddSeconds(1);

        reference.Id.Should().NotBe(Guid.Empty);
        reference.TenantId.Should().BeNull();
        reference.AuditEventId.Should().Be(Guid.Empty);
        reference.ResourceType.Should().BeEmpty();
        reference.ResourceId.Should().Be(Guid.Empty);
        reference.ResourceReference.Should().BeNull();
        reference.SourceService.Should().BeEmpty();
        reference.RelationshipType.Should().BeNull();
        reference.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void NavigationProperty_IsNullByDefault()
    {
        var reference = new AuditResourceReference();
        reference.AuditEvent.Should().BeNull();
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var eventId    = Guid.NewGuid();
        var resourceId = Guid.NewGuid();

        var reference = new AuditResourceReference
        {
            TenantId          = Guid.NewGuid(),
            AuditEventId      = eventId,
            ResourceType      = "Vehicle",
            ResourceId        = resourceId,
            ResourceReference = "VIN-123",
            SourceService     = "Fleet",
            RelationshipType  = "Subject"
        };

        reference.AuditEventId.Should().Be(eventId);
        reference.ResourceType.Should().Be("Vehicle");
        reference.ResourceId.Should().Be(resourceId);
        reference.ResourceReference.Should().Be("VIN-123");
        reference.SourceService.Should().Be("Fleet");
        reference.RelationshipType.Should().Be("Subject");
    }
}

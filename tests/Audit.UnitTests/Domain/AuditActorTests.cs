using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for <see cref="AuditActor"/>.
/// </summary>
public class AuditActorTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var actor  = new AuditActor();
        var after  = DateTime.UtcNow.AddSeconds(1);

        actor.Id.Should().NotBe(Guid.Empty);
        actor.TenantId.Should().BeNull();
        actor.ActorType.Should().BeEmpty();
        actor.ActorReference.Should().BeEmpty();
        actor.DisplayReference.Should().BeNull();
        actor.OrganizationReference.Should().BeNull();
        actor.ServiceName.Should().BeNull();
        actor.DeviceReference.Should().BeNull();
        actor.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var tenantId = Guid.NewGuid();
        var actor = new AuditActor
        {
            TenantId              = tenantId,
            ActorType             = "Service",
            ActorReference        = "svc-customs",
            DisplayReference      = "Customs Service",
            OrganizationReference = "org-1",
            ServiceName           = "Customs",
            DeviceReference       = "device-9"
        };

        actor.TenantId.Should().Be(tenantId);
        actor.ActorType.Should().Be("Service");
        actor.ActorReference.Should().Be("svc-customs");
        actor.DisplayReference.Should().Be("Customs Service");
        actor.OrganizationReference.Should().Be("org-1");
        actor.ServiceName.Should().Be("Customs");
        actor.DeviceReference.Should().Be("device-9");
    }
}

using Audit.Domain.Entities;
using FluentAssertions;

namespace Audit.UnitTests.Domain;

/// <summary>
/// Unit tests for the base audit ledger entity <see cref="AuditEvent"/>.
/// </summary>
public class AuditEventTests
{
    // ── Defaults ──────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_AssignsNewNonEmptyId()
    {
        var first  = new AuditEvent();
        var second = new AuditEvent();

        first.Id.Should().NotBe(Guid.Empty);
        second.Id.Should().NotBe(Guid.Empty);
        first.Id.Should().NotBe(second.Id);
    }

    [Fact]
    public void Constructor_SetsExpectedDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var evt = new AuditEvent();

        var after = DateTime.UtcNow.AddSeconds(1);

        evt.TenantId.Should().BeNull();
        evt.AuditReference.Should().BeEmpty();
        evt.EventType.Should().BeEmpty();
        evt.Category.Should().BeEmpty();
        evt.Severity.Should().Be("Informational");
        evt.ActorType.Should().BeEmpty();
        evt.ActorId.Should().BeNull();
        evt.ActorService.Should().BeNull();
        evt.ActorIpAddress.Should().BeNull();
        evt.ActorUserAgent.Should().BeNull();
        evt.SessionReference.Should().BeNull();
        evt.TenantContextSource.Should().BeNull();
        evt.ResourceType.Should().BeEmpty();
        evt.ResourceId.Should().BeNull();
        evt.ResourceReference.Should().BeNull();
        evt.Action.Should().BeEmpty();
        evt.Outcome.Should().BeEmpty();
        evt.Reason.Should().BeNull();
        evt.SourceService.Should().BeEmpty();
        evt.SourceEnvironment.Should().BeEmpty();
        evt.CorrelationId.Should().BeEmpty();
        evt.CausationId.Should().BeNull();
        evt.RequestId.Should().BeNull();
        evt.TraceId.Should().BeNull();
        evt.DataClassification.Should().Be("Internal");
        evt.IntegrityHash.Should().BeEmpty();
        evt.PreviousIntegrityHash.Should().BeNull();
        evt.SchemaVersion.Should().Be("1.0");

        // OccurredAtUtc has no initializer and must be set by the producer.
        evt.OccurredAtUtc.Should().Be(default(DateTime));
        evt.ReceivedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        evt.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    // ── Settable properties round-trip ────────────────────────────────────────

    [Fact]
    public void SettableProperties_ArePersisted()
    {
        var tenantId   = Guid.NewGuid();
        var actorId    = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var occurred   = new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var evt = new AuditEvent
        {
            TenantId              = tenantId,
            AuditReference        = "AUD-2026-000001",
            EventType             = "DeclarationCreated",
            Category              = "Customs",
            Severity              = "High",
            ActorType             = "User",
            ActorId               = actorId,
            Action                = "Create",
            Outcome               = "Success",
            ResourceType          = "Declaration",
            ResourceId            = resourceId,
            SourceService         = "Customs",
            SourceEnvironment     = "Production",
            CorrelationId         = "corr-123",
            OccurredAtUtc         = occurred,
            IntegrityHash         = "hash-current",
            PreviousIntegrityHash = "hash-previous"
        };

        evt.TenantId.Should().Be(tenantId);
        evt.AuditReference.Should().Be("AUD-2026-000001");
        evt.EventType.Should().Be("DeclarationCreated");
        evt.Category.Should().Be("Customs");
        evt.Severity.Should().Be("High");
        evt.ActorType.Should().Be("User");
        evt.ActorId.Should().Be(actorId);
        evt.Action.Should().Be("Create");
        evt.Outcome.Should().Be("Success");
        evt.ResourceType.Should().Be("Declaration");
        evt.ResourceId.Should().Be(resourceId);
        evt.SourceService.Should().Be("Customs");
        evt.SourceEnvironment.Should().Be("Production");
        evt.CorrelationId.Should().Be("corr-123");
        evt.OccurredAtUtc.Should().Be(occurred);
        evt.IntegrityHash.Should().Be("hash-current");
        evt.PreviousIntegrityHash.Should().Be("hash-previous");
    }
}

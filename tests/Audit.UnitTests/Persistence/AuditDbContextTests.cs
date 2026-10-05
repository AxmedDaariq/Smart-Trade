using Audit.Domain.Entities;
using Audit.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Audit.UnitTests.Persistence;

/// <summary>
/// Unit tests validating the EF Core model configuration of <see cref="AuditDbContext"/>.
/// The model is built without ever opening a database connection.
/// </summary>
public class AuditDbContextTests
{
    private static AuditDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AuditDbContext>()
            .UseNpgsql("Host=localhost;Database=smart_audit;Username=test;Password=test")
            .Options;

        return new AuditDbContext(options);
    }

    [Fact]
    public void Model_RegistersEveryAuditEntityType()
    {
        using var context = CreateContext();

        var expectedTypes = new[]
        {
            typeof(AuditEvent), typeof(AuditActor), typeof(AuditResourceReference),
            typeof(AuditChange), typeof(AuditApiRequest), typeof(AuditDataAccess),
            typeof(AuditSecurityEvent), typeof(AuditAccessEvent), typeof(AuditConfigurationChange),
            typeof(AuditApproval), typeof(AuditStatusHistory), typeof(AuditAlert),
            typeof(AuditIntegrityRecord), typeof(AuditRetentionPolicy), typeof(AuditLegalHold),
            typeof(AuditArchive), typeof(AuditExport), typeof(AuditExportItem),
            typeof(IdempotencyRecord), typeof(OutboxMessage), typeof(InboxMessage)
        };

        foreach (var type in expectedTypes)
        {
            context.Model.FindEntityType(type)
                .Should().NotBeNull($"entity {type.Name} should be mapped by the audit model");
        }
    }

    [Fact]
    public void AuditEvent_HasExpectedLedgerIndexes()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(AuditEvent))!;

        entityType.GetIndexes().Should().Contain(index =>
            index.Properties.Select(p => p.Name).SequenceEqual(new[] { "TenantId", "OccurredAtUtc" }));

        entityType.GetIndexes().Should().Contain(index =>
            index.IsUnique &&
            index.Properties.Select(p => p.Name).SequenceEqual(new[] { "TenantId", "AuditReference" }));

        entityType.GetIndexes().Should().Contain(index =>
            index.Properties.Select(p => p.Name).SequenceEqual(new[] { "PreviousIntegrityHash" }));
    }

    [Fact]
    public void AuditChange_SnapshotColumns_AreMappedAsJsonb()
    {
        using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(AuditChange))!;

        entityType.FindProperty(nameof(AuditChange.BeforeSnapshotJson))!.GetColumnType().Should().Be("jsonb");
        entityType.FindProperty(nameof(AuditChange.AfterSnapshotJson))!.GetColumnType().Should().Be("jsonb");
        entityType.FindProperty(nameof(AuditChange.ChangedFieldsJson))!.GetColumnType().Should().Be("jsonb");
    }

    [Fact]
    public void JsonPayloadColumns_AreMappedAsJsonb()
    {
        using var context = CreateContext();

        context.Model.FindEntityType(typeof(AuditExport))!
            .FindProperty(nameof(AuditExport.FilterJson))!.GetColumnType().Should().Be("jsonb");

        context.Model.FindEntityType(typeof(OutboxMessage))!
            .FindProperty(nameof(OutboxMessage.PayloadJson))!.GetColumnType().Should().Be("jsonb");
    }

    [Fact]
    public void UniqueIndexes_AreConfiguredForCrossTenantHygiene()
    {
        using var context = CreateContext();

        var alertType = context.Model.FindEntityType(typeof(AuditAlert))!;
        alertType.GetIndexes().Should().Contain(index =>
            index.IsUnique &&
            index.Properties.Select(p => p.Name).SequenceEqual(new[] { "TenantId", "AlertReference" }));

        var retentionType = context.Model.FindEntityType(typeof(AuditRetentionPolicy))!;
        retentionType.GetIndexes().Should().Contain(index =>
            index.IsUnique &&
            index.Properties.Select(p => p.Name).SequenceEqual(new[] { "TenantId", "Category" }));

        var idempotencyType = context.Model.FindEntityType(typeof(IdempotencyRecord))!;
        idempotencyType.GetIndexes().Should().Contain(index =>
            index.IsUnique &&
            index.Properties.Select(p => p.Name)
                .SequenceEqual(new[] { "TenantId", "IdempotencyKey", "Operation" }));
    }

    [Fact]
    public void Constructor_InitialisesDbSets()
    {
        using var context = CreateContext();

        context.AuditEvents.Should().NotBeNull();
        context.Actors.Should().NotBeNull();
        context.DataChanges.Should().NotBeNull();
        context.SecurityEvents.Should().NotBeNull();
        context.IntegrityRecords.Should().NotBeNull();
        context.LegalHolds.Should().NotBeNull();
        context.Archives.Should().NotBeNull();
        context.Exports.Should().NotBeNull();
        context.ExportItems.Should().NotBeNull();
        context.IdempotencyRecords.Should().NotBeNull();
        context.OutboxMessages.Should().NotBeNull();
        context.InboxMessages.Should().NotBeNull();
    }
}

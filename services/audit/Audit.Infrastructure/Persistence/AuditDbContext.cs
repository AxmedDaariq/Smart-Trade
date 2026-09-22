using Microsoft.EntityFrameworkCore;
using Audit.Domain.Entities;

namespace Audit.Infrastructure.Persistence;

public class AuditDbContext : DbContext
{
    // 1. Base Audit Ledger & Actors
    public DbSet<AuditEvent> AuditEvents { get; set; } = null!;
    public DbSet<AuditActor> Actors { get; set; } = null!;
    public DbSet<AuditResourceReference> ResourceReferences { get; set; } = null!;

    // 2. Deep Audit Types
    public DbSet<AuditChange> DataChanges { get; set; } = null!;
    public DbSet<AuditApiRequest> ApiRequests { get; set; } = null!;
    public DbSet<AuditDataAccess> DataAccessEvents { get; set; } = null!;
    public DbSet<AuditSecurityEvent> SecurityEvents { get; set; } = null!;
    public DbSet<AuditAccessEvent> AccessEvents { get; set; } = null!;
    public DbSet<AuditConfigurationChange> ConfigurationChanges { get; set; } = null!;
    public DbSet<AuditApproval> Approvals { get; set; } = null!;

    // 3. State & Alerts
    public DbSet<AuditStatusHistory> StatusHistories { get; set; } = null!;
    public DbSet<AuditAlert> Alerts { get; set; } = null!;

    // 4. Cryptographic Hash Chain
    public DbSet<AuditIntegrityRecord> IntegrityRecords { get; set; } = null!;

    // 5. Lifecycle & Compliance
    public DbSet<AuditRetentionPolicy> RetentionPolicies { get; set; } = null!;
    public DbSet<AuditLegalHold> LegalHolds { get; set; } = null!;
    public DbSet<AuditArchive> Archives { get; set; } = null!;

    // 6. Export Mechanism
    public DbSet<AuditExport> Exports { get; set; } = null!;
    public DbSet<AuditExportItem> ExportItems { get; set; } = null!;

    // 7. Resiliency & Messaging
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;

    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Base Audit Ledger
        modelBuilder.Entity<AuditEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.OccurredAtUtc });
            e.HasIndex(x => new { x.TenantId, x.AuditReference }).IsUnique();
            e.HasIndex(x => x.CorrelationId);
            e.HasIndex(x => x.TraceId);
            e.HasIndex(x => x.PreviousIntegrityHash); // Crucial for chaining
        });

        modelBuilder.Entity<AuditActor>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ActorType, x.ActorReference });
        });

        modelBuilder.Entity<AuditResourceReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.AuditEventId);
        });

        // 2. Deep Audit Types
        modelBuilder.Entity<AuditChange>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.AuditEventId).IsUnique(); // 1:1 or 1:Many depending on event size, assuming 1:1 for data changes
            e.Property(x => x.BeforeSnapshotJson).HasColumnType("jsonb");
            e.Property(x => x.AfterSnapshotJson).HasColumnType("jsonb");
            e.Property(x => x.ChangedFieldsJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<AuditApiRequest>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Route, x.StatusCode });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        modelBuilder.Entity<AuditDataAccess>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        modelBuilder.Entity<AuditSecurityEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.SecurityEventType, x.DetectedAtUtc });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        modelBuilder.Entity<AuditAccessEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.UserReference, x.ResourceType });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        modelBuilder.Entity<AuditConfigurationChange>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ConfigurationType, x.ConfigurationReference });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        modelBuilder.Entity<AuditApproval>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => x.AuditEventId).IsUnique();
        });

        // 3. State & Alerts
        modelBuilder.Entity<AuditStatusHistory>(e =>
        {
            e.HasIndex(x => x.AuditEventId);
            e.HasIndex(x => x.OccurredAtUtc);
        });

        modelBuilder.Entity<AuditAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.AlertReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.Status, x.DetectedAtUtc });
            e.HasIndex(x => x.SourceAuditEventId);
        });

        // 4. Cryptographic Hash Chain
        modelBuilder.Entity<AuditIntegrityRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CalculatedAtUtc });
            e.HasIndex(x => x.FirstAuditEventId);
            e.HasIndex(x => x.LastAuditEventId);
        });

        // 5. Lifecycle & Compliance
        modelBuilder.Entity<AuditRetentionPolicy>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Category }).IsUnique();
        });

        modelBuilder.Entity<AuditLegalHold>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Reference }).IsUnique();
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<AuditArchive>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ArchiveReference }).IsUnique();
            e.HasIndex(x => x.PeriodStartUtc);
        });

        // 6. Export Mechanism
        modelBuilder.Entity<AuditExport>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ExportReference }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.FilterJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<AuditExportItem>(e =>
        {
            e.HasIndex(x => new { x.ExportId, x.AuditEventId }).IsUnique();
        });

        // 7. Resiliency & Messaging
        modelBuilder.Entity<IdempotencyRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.IdempotencyKey, x.Operation }).IsUnique();
        });

        modelBuilder.Entity<InboxMessage>(e =>
        {
            e.HasIndex(x => new { x.MessageId, x.Consumer }).IsUnique();
        });

        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.OccurredAtUtc);
            e.Property(x => x.PayloadJson).HasColumnType("jsonb");
        });
    }
}

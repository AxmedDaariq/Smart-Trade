using Microsoft.EntityFrameworkCore;
using Declaration.Domain.Entities;
using BuildingBlocks.Persistence; // Ensure SmartDbContext is accessible

namespace Declaration.Infrastructure.Persistence;

public class DeclarationDbContext : SmartDbContext<DeclarationDbContext>
{
    public DeclarationDbContext(DbContextOptions<DeclarationDbContext> options) : base(options) { }

    public DbSet<CustomsDeclaration> CustomsDeclarations => Set<CustomsDeclaration>();
    public DbSet<DeclarationVersion> DeclarationVersions => Set<DeclarationVersion>();
    public DbSet<DeclarationReference> DeclarationReferences => Set<DeclarationReference>();
    public DbSet<DeclarationParty> DeclarationParties => Set<DeclarationParty>();
    public DbSet<DeclarationPartyRole> DeclarationPartyRoles => Set<DeclarationPartyRole>();
    
    public DbSet<DeclarationCargo> DeclarationCargos => Set<DeclarationCargo>();
    public DbSet<DeclarationCargoItem> DeclarationCargoItems => Set<DeclarationCargoItem>();
    public DbSet<DeclarationContainer> DeclarationContainers => Set<DeclarationContainer>();
    public DbSet<DeclarationTransport> DeclarationTransports => Set<DeclarationTransport>();
    public DbSet<DeclarationRoute> DeclarationRoutes => Set<DeclarationRoute>();
    
    public DbSet<DeclarationDocument> DeclarationDocuments => Set<DeclarationDocument>();
    public DbSet<DeclarationDocumentVersion> DeclarationDocumentVersions => Set<DeclarationDocumentVersion>();
    
    public DbSet<DeclarationValidation> DeclarationValidations => Set<DeclarationValidation>();
    public DbSet<DeclarationAmendment> DeclarationAmendments => Set<DeclarationAmendment>();
    public DbSet<DeclarationSubmission> DeclarationSubmissions => Set<DeclarationSubmission>();
    public DbSet<DeclarationDecision> DeclarationDecisions => Set<DeclarationDecision>();
    public DbSet<DeclarationStatusHistory> DeclarationStatusHistories => Set<DeclarationStatusHistory>();
    
    public DbSet<DeclarationExternalReference> DeclarationExternalReferences => Set<DeclarationExternalReference>();
    public DbSet<DeclarationIntegrationRecord> DeclarationIntegrationRecords => Set<DeclarationIntegrationRecord>();
    public DbSet<DeclarationAuditEntry> DeclarationAuditEntries => Set<DeclarationAuditEntry>();
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Core Aggregate
        builder.Entity<CustomsDeclaration>(e => 
        {
            e.HasIndex(d => new { d.TenantId, d.DeclarationNumber }).IsUnique();
            e.HasIndex(d => d.OrganizationId);
            e.HasIndex(d => new { d.TenantId, d.Status });
            e.HasIndex(d => new { d.TenantId, d.CreatedAtUtc });
            e.HasIndex(d => d.CurrentVersionNumber);
            e.Property(d => d.Version).IsRowVersion();
            
            // Restrict Cascade Delete
            e.HasMany(d => d.Versions).WithOne(v => v.CustomsDeclaration).HasForeignKey(v => v.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.Validations).WithOne(v => v.CustomsDeclaration).HasForeignKey(v => v.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.Submissions).WithOne(s => s.CustomsDeclaration).HasForeignKey(s => s.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.Decisions).WithOne(dec => dec.CustomsDeclaration).HasForeignKey(dec => dec.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.Amendments).WithOne(a => a.CustomsDeclaration).HasForeignKey(a => a.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.StatusHistories).WithOne(sh => sh.CustomsDeclaration).HasForeignKey(sh => sh.DeclarationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(d => d.References).WithOne(r => r.CustomsDeclaration).HasForeignKey(r => r.DeclarationId).OnDelete(DeleteBehavior.Restrict);
        });

        // Declaration Version
        builder.Entity<DeclarationVersion>(e =>
        {
            e.HasIndex(v => new { v.DeclarationId, v.VersionNumber }).IsUnique();
            e.HasIndex(v => v.VersionStatus);
            e.Property(v => v.Version).IsRowVersion();
            
            e.Property(v => v.TotalDeclaredValue).HasColumnType("numeric(19,4)");
            e.Property(v => v.TotalGrossWeight).HasColumnType("numeric(19,4)");
            e.Property(v => v.TotalNetWeight).HasColumnType("numeric(19,4)");
            
            e.HasMany(v => v.Parties).WithOne(p => p.DeclarationVersion).HasForeignKey(p => p.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(v => v.Cargos).WithOne(c => c.DeclarationVersion).HasForeignKey(c => c.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(v => v.Containers).WithOne(c => c.DeclarationVersion).HasForeignKey(c => c.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(v => v.Transports).WithOne(t => t.DeclarationVersion).HasForeignKey(t => t.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(v => v.Routes).WithOne(r => r.DeclarationVersion).HasForeignKey(r => r.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(v => v.Documents).WithOne(d => d.DeclarationVersion).HasForeignKey(d => d.DeclarationVersionId).OnDelete(DeleteBehavior.Cascade);
        });

        // References
        builder.Entity<DeclarationReference>(e =>
        {
            e.HasIndex(r => new { r.DeclarationId, r.ReferenceType, r.NormalizedValue });
            e.HasIndex(r => r.TenantId);
        });

        // Cargo
        builder.Entity<DeclarationCargo>(e =>
        {
            e.HasIndex(c => c.CommodityCode);
            e.Property(c => c.GrossWeight).HasColumnType("numeric(19,4)");
            e.Property(c => c.NetWeight).HasColumnType("numeric(19,4)");
            e.Property(c => c.Volume).HasColumnType("numeric(19,4)");
            e.Property(c => c.DeclaredValue).HasColumnType("numeric(19,4)");
            
            e.HasMany(c => c.Items).WithOne(i => i.DeclarationCargo).HasForeignKey(i => i.DeclarationCargoId).OnDelete(DeleteBehavior.Cascade);
        });

        // Cargo Item
        builder.Entity<DeclarationCargoItem>(e =>
        {
            e.HasIndex(i => new { i.DeclarationCargoId, i.LineNumber }).IsUnique();
            e.HasIndex(i => i.HsCodeDeclared);
            e.HasIndex(i => i.CommodityCode);
            e.HasIndex(i => i.CountryOfOrigin);
            
            e.Property(i => i.Quantity).HasColumnType("numeric(19,4)");
            e.Property(i => i.GrossWeight).HasColumnType("numeric(19,4)");
            e.Property(i => i.NetWeight).HasColumnType("numeric(19,4)");
            e.Property(i => i.UnitPrice).HasColumnType("numeric(19,4)");
            e.Property(i => i.TotalValue).HasColumnType("numeric(19,4)");
        });

        // Container
        builder.Entity<DeclarationContainer>(e =>
        {
            e.HasIndex(c => new { c.DeclarationVersionId, c.NormalizedContainerNumber }).IsUnique();
            e.Property(c => c.GrossWeight).HasColumnType("numeric(19,4)");
        });

        // Party
        builder.Entity<DeclarationParty>(e =>
        {
            e.HasMany(p => p.Roles).WithOne(r => r.DeclarationParty).HasForeignKey(r => r.DeclarationPartyId).OnDelete(DeleteBehavior.Cascade);
        });

        // Document
        builder.Entity<DeclarationDocument>(e =>
        {
            e.HasIndex(d => d.DocumentType);
            e.HasIndex(d => d.Status);
            e.Property(d => d.Version).IsRowVersion();
            
            e.HasMany(d => d.DocumentVersions).WithOne(v => v.DeclarationDocument).HasForeignKey(v => v.DeclarationDocumentId).OnDelete(DeleteBehavior.Cascade);
        });

        // Submissions
        builder.Entity<DeclarationSubmission>(e =>
        {
            e.HasIndex(s => s.SubmissionReference);
            e.HasIndex(s => s.SubmittedAtUtc);
        });

        // Amendments
        builder.Entity<DeclarationAmendment>(e =>
        {
            e.HasIndex(a => a.Status);
            e.HasIndex(a => a.RequestedAtUtc);
            e.Property(a => a.ChangeSummaryJson).HasColumnType("jsonb");
            e.Property(a => a.Version).IsRowVersion();
        });

        // Validation
        builder.Entity<DeclarationValidation>(e =>
        {
            e.Property(v => v.DetailsJson).HasColumnType("jsonb");
        });

        // Audit
        builder.Entity<DeclarationAuditEntry>(e =>
        {
            e.Property(a => a.BeforeJson).HasColumnType("jsonb");
            e.Property(a => a.AfterJson).HasColumnType("jsonb");
        });

        // External Reference
        builder.Entity<DeclarationExternalReference>(e =>
        {
            e.HasIndex(er => new { er.ExternalSystem, er.ExternalReference });
        });

        // Idempotency
        builder.Entity<IdempotencyRecord>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.IdempotencyKey, i.Operation }).IsUnique();
            e.Property(i => i.ResponseBodyJson).HasColumnType("jsonb");
        });

        // Inbox / Outbox
        builder.Entity<InboxMessage>(e =>
        {
            e.HasIndex(i => new { i.MessageId, i.Consumer }).IsUnique();
        });
        
        builder.Entity<OutboxMessage>(e =>
        {
            e.HasIndex(o => o.Status);
            e.HasIndex(o => o.OccurredAtUtc);
            e.Property(o => o.PayloadJson).HasColumnType("jsonb");
        });
    }
}

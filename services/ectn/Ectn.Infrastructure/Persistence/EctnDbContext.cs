using Microsoft.EntityFrameworkCore;
using Ectn.Domain.Entities;
using BuildingBlocks.Persistence; // Ensure SmartDbContext is accessible

namespace Ectn.Infrastructure.Persistence;

public class EctnDbContext : SmartDbContext
{
    public EctnDbContext(DbContextOptions<EctnDbContext> options) : base(options) { }

    public DbSet<EctnApplication> EctnApplications => Set<EctnApplication>();
    public DbSet<EctnCertificate> EctnCertificates => Set<EctnCertificate>();
    public DbSet<EctnReference> EctnReferences => Set<EctnReference>();
    public DbSet<EctnParty> EctnParties => Set<EctnParty>();
    public DbSet<EctnPartyRole> EctnPartyRoles => Set<EctnPartyRole>();
    
    public DbSet<EctnShipment> EctnShipments => Set<EctnShipment>();
    public DbSet<EctnCargo> EctnCargos => Set<EctnCargo>();
    public DbSet<EctnCargoItem> EctnCargoItems => Set<EctnCargoItem>();
    public DbSet<EctnContainer> EctnContainers => Set<EctnContainer>();
    public DbSet<EctnBillOfLading> EctnBillOfLadings => Set<EctnBillOfLading>();
    public DbSet<EctnVesselReference> EctnVesselReferences => Set<EctnVesselReference>();
    public DbSet<EctnVoyageReference> EctnVoyageReferences => Set<EctnVoyageReference>();
    
    public DbSet<EctnValidation> EctnValidations => Set<EctnValidation>();
    public DbSet<EctnReview> EctnReviews => Set<EctnReview>();
    public DbSet<EctnDecision> EctnDecisions => Set<EctnDecision>();
    public DbSet<EctnAmendment> EctnAmendments => Set<EctnAmendment>();
    public DbSet<EctnStatusHistory> EctnStatusHistories => Set<EctnStatusHistory>();
    
    public DbSet<EctnDocument> EctnDocuments => Set<EctnDocument>();
    public DbSet<EctnDocumentVersion> EctnDocumentVersions => Set<EctnDocumentVersion>();
    public DbSet<EctnDocumentVerification> EctnDocumentVerifications => Set<EctnDocumentVerification>();
    
    public DbSet<EctnQrCode> EctnQrCodes => Set<EctnQrCode>();
    public DbSet<EctnVerification> EctnVerifications => Set<EctnVerification>();
    
    public DbSet<EctnExternalReference> EctnExternalReferences => Set<EctnExternalReference>();
    public DbSet<EctnIntegrationRecord> EctnIntegrationRecords => Set<EctnIntegrationRecord>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<EctnAuditEntry> EctnAuditEntries => Set<EctnAuditEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Core Aggregate
        builder.Entity<EctnApplication>(e => 
        {
            e.HasIndex(a => new { a.TenantId, a.ApplicationNumber }).IsUnique();
            e.HasIndex(a => a.OrganizationId);
            e.HasIndex(a => a.Status);
            e.HasIndex(a => a.CreatedAtUtc);
            e.Property(a => a.Version).IsRowVersion();
            
            // Restrict Cascade Delete - historical ECTNs shouldn't be deleted implicitly
            e.HasMany(a => a.Certificates).WithOne(c => c.EctnApplication).HasForeignKey(c => c.EctnApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(a => a.Shipments).WithOne(s => s.EctnApplication).HasForeignKey(s => s.EctnApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(a => a.PartyRoles).WithOne(pr => pr.EctnApplication).HasForeignKey(pr => pr.EctnApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(a => a.Documents).WithOne(d => d.EctnApplication).HasForeignKey(d => d.EctnApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(a => a.StatusHistories).WithOne(sh => sh.EctnApplication).HasForeignKey(sh => sh.EctnApplicationId).OnDelete(DeleteBehavior.Restrict);
        });

        // Certificate
        builder.Entity<EctnCertificate>(e =>
        {
            e.HasIndex(c => c.EctnNumber).IsUnique(); // Policy rule
            e.HasIndex(c => c.CertificateNumber).IsUnique();
            e.HasIndex(c => c.Status);
            e.HasIndex(c => c.ExpiresAtUtc);
        });

        // References
        builder.Entity<EctnReference>(e =>
        {
            e.HasIndex(r => new { r.TenantId, r.ReferenceType, r.NormalizedValue }).IsUnique();
            e.HasOne(r => r.EctnApplication).WithMany().HasForeignKey(r => r.EctnApplicationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Party Roles
        builder.Entity<EctnPartyRole>(e =>
        {
            e.HasIndex(pr => new { pr.EctnApplicationId, pr.EctnPartyId, pr.RoleType }).IsUnique();
            e.HasOne(pr => pr.EctnParty).WithMany().HasForeignKey(pr => pr.EctnPartyId).OnDelete(DeleteBehavior.Restrict);
        });

        // Shipment
        builder.Entity<EctnShipment>(e =>
        {
            e.HasIndex(s => s.ShipmentReference);
            
            // Numeric configurations
            e.Property(s => s.TotalGrossWeight).HasColumnType("numeric(18,2)");
            e.Property(s => s.TotalNetWeight).HasColumnType("numeric(18,2)");
            e.Property(s => s.TotalVolume).HasColumnType("numeric(18,2)");
            e.Property(s => s.DeclaredValue).HasColumnType("numeric(18,2)");
            e.Property(s => s.FreightValue).HasColumnType("numeric(18,2)");
            e.Property(s => s.InsuranceValue).HasColumnType("numeric(18,2)");
            
            e.HasMany(s => s.Cargos).WithOne(c => c.EctnShipment).HasForeignKey(c => c.EctnShipmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(s => s.Containers).WithOne(c => c.EctnShipment).HasForeignKey(c => c.EctnShipmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(s => s.BillOfLadings).WithOne(b => b.EctnShipment).HasForeignKey(b => b.EctnShipmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(s => s.VesselReferences).WithOne(v => v.EctnShipment).HasForeignKey(v => v.EctnShipmentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(s => s.VoyageReferences).WithOne(v => v.EctnShipment).HasForeignKey(v => v.EctnShipmentId).OnDelete(DeleteBehavior.Cascade);
        });

        // Cargo
        builder.Entity<EctnCargo>(e =>
        {
            e.HasIndex(c => c.HsCode);
            e.HasIndex(c => c.CommodityCode);
            
            e.Property(c => c.GrossWeight).HasColumnType("numeric(18,2)");
            e.Property(c => c.NetWeight).HasColumnType("numeric(18,2)");
            e.Property(c => c.Volume).HasColumnType("numeric(18,2)");
            e.Property(c => c.DeclaredValue).HasColumnType("numeric(18,2)");
            
            e.HasMany(c => c.Items).WithOne(i => i.EctnCargo).HasForeignKey(i => i.EctnCargoId).OnDelete(DeleteBehavior.Cascade);
        });

        // Cargo Item
        builder.Entity<EctnCargoItem>(e =>
        {
            e.HasIndex(i => new { i.EctnCargoId, i.LineNumber }).IsUnique();
            
            e.Property(i => i.Quantity).HasColumnType("numeric(18,4)");
            e.Property(i => i.GrossWeight).HasColumnType("numeric(18,2)");
            e.Property(i => i.NetWeight).HasColumnType("numeric(18,2)");
            e.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
            e.Property(i => i.TotalValue).HasColumnType("numeric(18,2)");
        });

        // Container
        builder.Entity<EctnContainer>(e =>
        {
            e.HasIndex(c => c.NormalizedContainerNumber);
            e.Property(c => c.GrossWeight).HasColumnType("numeric(18,2)");
        });

        // Bill of Lading
        builder.Entity<EctnBillOfLading>(e =>
        {
            e.HasIndex(b => new { b.EctnShipmentId, b.NormalizedBolNumber }).IsUnique();
        });

        // Validation
        builder.Entity<EctnValidation>(e =>
        {
            e.Property(v => v.ValidationDetailsJson).HasColumnType("jsonb");
            e.HasOne(v => v.EctnApplication).WithMany().HasForeignKey(v => v.EctnApplicationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Amendment
        builder.Entity<EctnAmendment>(e =>
        {
            e.Property(a => a.ChangeSummaryJson).HasColumnType("jsonb");
            e.Property(a => a.Version).IsRowVersion();
            e.HasOne(a => a.EctnApplication).WithMany().HasForeignKey(a => a.EctnApplicationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Documents
        builder.Entity<EctnDocument>(e =>
        {
            e.HasIndex(d => d.Status);
            e.HasIndex(d => d.DocumentType);
            e.Property(d => d.Version).IsRowVersion();
            
            e.HasMany(d => d.DocumentVersions).WithOne(v => v.EctnDocument).HasForeignKey(v => v.EctnDocumentId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(d => d.Verifications).WithOne(v => v.EctnDocument).HasForeignKey(v => v.EctnDocumentId).OnDelete(DeleteBehavior.Cascade);
        });

        // Audit & Integration
        builder.Entity<EctnAuditEntry>(e =>
        {
            e.Property(a => a.BeforeJson).HasColumnType("jsonb");
            e.Property(a => a.AfterJson).HasColumnType("jsonb");
        });

        builder.Entity<IdempotencyRecord>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.IdempotencyKey, i.Operation }).IsUnique();
            e.Property(i => i.ResponseBodyJson).HasColumnType("jsonb");
        });

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
        
        builder.Entity<EctnExternalReference>(e =>
        {
            e.HasIndex(er => new { er.ExternalSystem, er.ExternalReference });
            e.HasIndex(er => er.ExternalStatus);
            e.HasOne(er => er.EctnApplication).WithMany().HasForeignKey(er => er.EctnApplicationId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

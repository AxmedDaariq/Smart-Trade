using Microsoft.EntityFrameworkCore;
using Fleet.Domain.Entities;
using BuildingBlocks.Persistence;

namespace Fleet.Infrastructure.Persistence;

public class FleetDbContext : SmartDbContext<FleetDbContext>
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

    // Core
    public DbSet<FleetCompany> FleetCompanies => Set<FleetCompany>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Truck> Trucks => Set<Truck>();
    public DbSet<Trailer> Trailers => Set<Trailer>();
    public DbSet<Driver> Drivers => Set<Driver>();

    // Compliance & Licensing
    public DbSet<VehicleRegistration> Registrations => Set<VehicleRegistration>();
    public DbSet<VehicleCompliance> VehicleCompliances => Set<VehicleCompliance>();
    public DbSet<DriverLicense> Licenses => Set<DriverLicense>();
    public DbSet<DriverQualification> Qualifications => Set<DriverQualification>();
    public DbSet<DriverCompliance> DriverCompliances => Set<DriverCompliance>();
    public DbSet<VehicleInspection> Inspections => Set<VehicleInspection>();
    public DbSet<DriverVerification> Verifications => Set<DriverVerification>();

    // Documents & Capabilities
    public DbSet<VehicleDocument> VehicleDocuments => Set<VehicleDocument>();
    public DbSet<DriverDocument> DriverDocuments => Set<DriverDocument>();
    public DbSet<FleetDocumentVersion> DocumentVersions => Set<FleetDocumentVersion>();
    public DbSet<VehicleCapability> VehicleCapabilities => Set<VehicleCapability>();
    public DbSet<DriverCapability> DriverCapabilities => Set<DriverCapability>();
    public DbSet<VehicleMaintenanceRecord> MaintenanceRecords => Set<VehicleMaintenanceRecord>();

    // Assignments & Dispatch
    public DbSet<VehicleAssignment> VehicleAssignments => Set<VehicleAssignment>();
    public DbSet<DriverAssignment> DriverAssignments => Set<DriverAssignment>();
    public DbSet<TrailerAssignment> TrailerAssignments => Set<TrailerAssignment>();
    public DbSet<DispatchOrder> DispatchOrders => Set<DispatchOrder>();
    public DbSet<DispatchRequirement> DispatchRequirements => Set<DispatchRequirement>();

    // History & Holds
    public DbSet<FleetHold> Holds => Set<FleetHold>();
    public DbSet<DispatchStatusHistory> DispatchStatusHistories => Set<DispatchStatusHistory>();
    public DbSet<VehicleStatusHistory> VehicleStatusHistories => Set<VehicleStatusHistory>();
    public DbSet<DriverStatusHistory> DriverStatusHistories => Set<DriverStatusHistory>();
    public DbSet<AssignmentHistory> AssignmentHistories => Set<AssignmentHistory>();

    // Integration & Audit
    public DbSet<FleetExternalReference> ExternalReferences => Set<FleetExternalReference>();
    public DbSet<FleetIntegrationRecord> IntegrationRecords => Set<FleetIntegrationRecord>();
    public DbSet<FleetAuditEntry> AuditEntries => Set<FleetAuditEntry>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Core
        builder.Entity<FleetCompany>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.OrganizationId }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.FleetCode }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<Vehicle>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.NormalizedLicensePlate }).IsUnique();
            e.HasIndex(x => x.VIN);
            e.HasIndex(x => new { x.TenantId, x.FleetCompanyId, x.Status });
            e.Property(x => x.Version).IsRowVersion();

            e.HasOne(x => x.Truck)
             .WithOne(t => t.Vehicle)
             .HasForeignKey<Truck>(t => t.VehicleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Truck>(e =>
        {
            e.Property(x => x.GrossVehicleWeight).HasColumnType("numeric(19,4)");
            e.Property(x => x.TareWeight).HasColumnType("numeric(19,4)");
            e.Property(x => x.PayloadCapacity).HasColumnType("numeric(19,4)");
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<Trailer>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TrailerReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.NormalizedLicensePlate }).IsUnique();
            
            e.Property(x => x.GrossVehicleWeight).HasColumnType("numeric(19,4)");
            e.Property(x => x.TareWeight).HasColumnType("numeric(19,4)");
            e.Property(x => x.PayloadCapacity).HasColumnType("numeric(19,4)");
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<Driver>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.DriverReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.UserId }); // UserId may be null for offline drivers
            e.HasIndex(x => new { x.TenantId, x.FleetCompanyId, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        // Assignments (Concurrency protected)
        builder.Entity<VehicleAssignment>(e =>
        {
            e.HasIndex(x => new { x.VehicleId, x.Status, x.StartAtUtc, x.EndAtUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<DriverAssignment>(e =>
        {
            e.HasIndex(x => new { x.DriverId, x.Status, x.StartAtUtc, x.EndAtUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<TrailerAssignment>(e =>
        {
            e.HasIndex(x => new { x.TrailerId, x.Status, x.StartAtUtc, x.EndAtUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        // Dispatch
        builder.Entity<DispatchOrder>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.FleetCompanyId, x.DispatchReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.VehicleId, x.DriverId, x.Status });
            e.Property(x => x.Version).IsRowVersion();
            
            e.HasMany(x => x.Requirements)
             .WithOne(r => r.DispatchOrder)
             .HasForeignKey(r => r.DispatchOrderId)
             .OnDelete(DeleteBehavior.Cascade);
             
            e.HasMany(x => x.DriverAssignments).WithOne(a => a.DispatchOrder).HasForeignKey(a => a.DispatchOrderId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.TrailerAssignments).WithOne(a => a.DispatchOrder).HasForeignKey(a => a.DispatchOrderId).OnDelete(DeleteBehavior.Restrict);
        });

        // Compliance & Inspections
        builder.Entity<VehicleCompliance>(e =>
        {
            e.HasIndex(x => new { x.VehicleId, x.Status });
            e.HasIndex(x => new { x.VehicleId, x.ExpiryDateUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        builder.Entity<DriverCompliance>(e =>
        {
            e.HasIndex(x => new { x.DriverId, x.Status });
            e.HasIndex(x => new { x.DriverId, x.ExpiryDateUtc });
            e.Property(x => x.Version).IsRowVersion();
        });
        
        // Documents
        builder.Entity<VehicleDocument>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleId, x.DocumentType, x.Status });
        });

        // Integration / Idempotency / Audit / Inbox / Outbox
        builder.Entity<FleetAuditEntry>(e =>
        {
            e.Property(x => x.BeforeJson).HasColumnType("jsonb");
            e.Property(x => x.AfterJson).HasColumnType("jsonb");
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
        });

        builder.Entity<AssignmentHistory>(e =>
        {
            e.Property(x => x.PreviousValueJson).HasColumnType("jsonb");
            e.Property(x => x.NewValueJson).HasColumnType("jsonb");
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
        
        builder.Entity<FleetHold>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId, x.Status });
        });
        
        builder.Entity<FleetExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });
    }
}

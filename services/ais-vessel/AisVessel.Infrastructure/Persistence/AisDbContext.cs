using Microsoft.EntityFrameworkCore;
using AisVessel.Domain.Entities;

namespace AisVessel.Infrastructure.Persistence;

public class AisDbContext : DbContext
{
    // 1. Core Vessel Entities
    public DbSet<Vessel> Vessels { get; set; } = null!;
    public DbSet<VesselIdentifier> VesselIdentifiers { get; set; } = null!;
    public DbSet<VesselCharacteristic> VesselCharacteristics { get; set; } = null!;
    public DbSet<VesselNavigationStatus> VesselNavigationStatuses { get; set; } = null!;

    // 2. AIS Tracking & Devices
    public DbSet<AisDevice> AisDevices { get; set; } = null!;
    public DbSet<AisProvider> AisProviders { get; set; } = null!;
    public DbSet<AisDeviceAssignment> AisDeviceAssignments { get; set; } = null!;
    public DbSet<AisPosition> AisPositions { get; set; } = null!;
    public DbSet<VesselTrackingSession> VesselTrackingSessions { get; set; } = null!;

    // 3. Voyage & Port Call
    public DbSet<Voyage> Voyages { get; set; } = null!;
    public DbSet<PortCall> PortCalls { get; set; } = null!;
    public DbSet<Port> Ports { get; set; } = null!;
    public DbSet<PortFacility> PortFacilities { get; set; } = null!;
    public DbSet<Berth> Berths { get; set; } = null!;
    public DbSet<AnchorageArea> AnchorageAreas { get; set; } = null!;
    public DbSet<BerthAssignment> BerthAssignments { get; set; } = null!;
    public DbSet<VesselPortEvent> VesselPortEvents { get; set; } = null!;
    public DbSet<PortOperationalStatus> PortOperationalStatuses { get; set; } = null!;

    // 4. ETA & Delays
    public DbSet<VesselEtaRecord> VesselEtaRecords { get; set; } = null!;
    public DbSet<VesselDelay> VesselDelays { get; set; } = null!;

    // 5. Alerts & Quality
    public DbSet<AisAlert> AisAlerts { get; set; } = null!;
    public DbSet<AisDataQualityEvent> AisDataQualityEvents { get; set; } = null!;

    // 6. Integration Boundaries
    public DbSet<AisExternalReference> AisExternalReferences { get; set; } = null!;
    public DbSet<AisIntegrationRecord> AisIntegrationRecords { get; set; } = null!;
    public DbSet<AisAuditEntry> AisAuditEntries { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;

    public AisDbContext(DbContextOptions<AisDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Setup PostGIS extension
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Vessel>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselReference }).IsUnique();
            e.HasIndex(x => x.ImoNumber);
            e.HasIndex(x => x.MmsiNumber);
            e.HasIndex(x => x.Status);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<VesselIdentifier>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId });
        });

        modelBuilder.Entity<VesselCharacteristic>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId });
        });

        modelBuilder.Entity<VesselNavigationStatus>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId });
        });

        modelBuilder.Entity<AisDevice>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.DeviceReference }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<AisProvider>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ProviderReference }).IsUnique();
        });

        modelBuilder.Entity<AisDeviceAssignment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.AisDeviceId, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<AisPosition>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.AisDeviceId, x.TimestampUtc, x.MmsiNumber });
            e.HasIndex(x => x.Position).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<VesselTrackingSession>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.Status });
            e.Property(x => x.StartPosition).HasColumnType("geography(Point,4326)");
            e.Property(x => x.EndPosition).HasColumnType("geography(Point,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<Voyage>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VoyageReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.Status, x.PlannedArrivalAtUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<PortCall>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortId, x.VesselId, x.PortCallReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.EstimatedArrivalAtUtc, x.ActualArrivalAtUtc });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<Port>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortReference }).IsUnique();
            e.HasIndex(x => x.PortCode);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Location).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Location).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.BoundaryGeometry).HasMethod("GIST");
            e.Property(x => x.BoundaryGeometry).HasColumnType("geometry(Geometry,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<PortFacility>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortId });
            e.HasIndex(x => x.Location).HasMethod("GIST");
            e.Property(x => x.Location).HasColumnType("geography(Point,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<Berth>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortId, x.BerthReference }).IsUnique();
            e.HasIndex(x => x.Geometry).HasMethod("GIST");
            e.Property(x => x.Geometry).HasColumnType("geometry(Geometry,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<AnchorageArea>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortId, x.AnchorageReference }).IsUnique();
            e.HasIndex(x => x.Geometry).HasMethod("GIST");
            e.Property(x => x.Geometry).HasColumnType("geometry(Geometry,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<BerthAssignment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.BerthId, x.PortCallId, x.Status, x.PlannedStartAtUtc });
        });

        modelBuilder.Entity<VesselPortEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortCallId, x.VesselId, x.OccurredAtUtc });
            e.HasIndex(x => x.Position).HasMethod("GIST");
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<PortOperationalStatus>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PortId, x.Status });
        });

        modelBuilder.Entity<VesselEtaRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.VoyageId, x.PortCallId });
        });

        modelBuilder.Entity<VesselDelay>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.VoyageId, x.PortCallId, x.Status });
        });

        modelBuilder.Entity<AisAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.Severity, x.Status, x.OccurredAtUtc });
            e.HasIndex(x => x.Position).HasMethod("GIST");
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<AisDataQualityEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VesselId, x.OccurredAtUtc });
        });

        modelBuilder.Entity<AisExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });

        modelBuilder.Entity<AisIntegrationRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
        });

        modelBuilder.Entity<AisAuditEntry>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.Property(x => x.BeforeJson).HasColumnType("jsonb");
            e.Property(x => x.AfterJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<IdempotencyRecord>(e =>
        {
            e.HasIndex(i => new { i.TenantId, i.IdempotencyKey, i.Operation }).IsUnique();
            e.Property(i => i.ResponseBodyJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<InboxMessage>(e =>
        {
            e.HasIndex(i => new { i.MessageId, i.Consumer }).IsUnique();
        });
        
        modelBuilder.Entity<OutboxMessage>(e =>
        {
            e.HasIndex(o => o.Status);
            e.HasIndex(o => o.OccurredAtUtc);
            e.Property(o => o.PayloadJson).HasColumnType("jsonb");
        });
    }
}

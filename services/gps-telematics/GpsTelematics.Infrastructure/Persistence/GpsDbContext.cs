using Microsoft.EntityFrameworkCore;
using GpsTelematics.Domain.Entities;

namespace GpsTelematics.Infrastructure.Persistence;

public class GpsDbContext : DbContext
{
    // PostgreSQL Core Entities
    public DbSet<GpsDevice> Devices { get; set; } = null!;
    public DbSet<TelematicsDevice> TelematicsDevices { get; set; } = null!;
    public DbSet<DeviceAssignment> DeviceAssignments { get; set; } = null!;
    public DbSet<DeviceConnection> DeviceConnections { get; set; } = null!;
    public DbSet<TrackingSession> TrackingSessions { get; set; } = null!;
    public DbSet<RouteTrackingSession> RouteTrackingSessions { get; set; } = null!;
    public DbSet<VehicleLocation> VehicleLocations { get; set; } = null!;
    public DbSet<LocationStatusHistory> LocationStatusHistories { get; set; } = null!;
    public DbSet<GpsPosition> GpsPositions { get; set; } = null!;
    public DbSet<DeviceHealth> DeviceHealths { get; set; } = null!;

    // Geofence & Alert Entities
    public DbSet<Geofence> Geofences { get; set; } = null!;
    public DbSet<GeofenceRule> GeofenceRules { get; set; } = null!;
    public DbSet<GeofenceEvent> GeofenceEvents { get; set; } = null!;
    public DbSet<TrackingAlert> TrackingAlerts { get; set; } = null!;
    public DbSet<DeviceAlert> DeviceAlerts { get; set; } = null!;

    // Command & Integration Entities
    public DbSet<DeviceCommand> DeviceCommands { get; set; } = null!;
    public DbSet<DeviceCommandResult> DeviceCommandResults { get; set; } = null!;
    public DbSet<GpsExternalReference> ExternalReferences { get; set; } = null!;
    public DbSet<GpsIntegrationRecord> IntegrationRecords { get; set; } = null!;
    public DbSet<GpsAuditEntry> AuditEntries { get; set; } = null!;
    
    // Outbox/Inbox
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;

    // Telemetry Entities (Mapped for SQL queryability, even if high-volume hits MongoDB)
    public DbSet<OdometerReading> OdometerReadings { get; set; } = null!;
    public DbSet<EngineStatus> EngineStatuses { get; set; } = null!;
    public DbSet<FuelReading> FuelReadings { get; set; } = null!;
    public DbSet<SensorReading> SensorReadings { get; set; } = null!;

    public GpsDbContext(DbContextOptions<GpsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Setup PostGIS extension
        modelBuilder.HasPostgresExtension("postgis");

        // 1. PostgreSQL Core Entities
        modelBuilder.Entity<GpsDevice>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.DeviceReference }).IsUnique();
            e.HasIndex(x => x.DeviceSerialNumber);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.LastSeenAtUtc);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<TelematicsDevice>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<DeviceAssignment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.VehicleReference, x.StartAtUtc, x.EndAtUtc, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<DeviceConnection>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.Status });
        });

        modelBuilder.Entity<TrackingSession>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.VehicleReference, x.Status, x.StartedAtUtc });
            e.Property(x => x.StartPosition).HasColumnType("geography(Point,4326)");
            e.Property(x => x.EndPosition).HasColumnType("geography(Point,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<RouteTrackingSession>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.VehicleReference, x.Status });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<VehicleLocation>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.TimestampUtc });
            e.HasIndex(x => x.Position).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<LocationStatusHistory>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference });
        });

        modelBuilder.Entity<GpsPosition>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.GpsDeviceId, x.TimestampUtc, x.TrackingSessionId });
            e.HasIndex(x => x.Position).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<DeviceHealth>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId });
        });

        // 2. Geofence & Alert Entities
        modelBuilder.Entity<Geofence>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.Status });
            e.HasIndex(x => x.Geometry).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Geometry).HasColumnType("geometry(Geometry,4326)");
            e.HasIndex(x => x.CenterPoint).HasMethod("GIST");
            e.Property(x => x.CenterPoint).HasColumnType("geography(Point,4326)");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<GeofenceRule>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GeofenceId });
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<GeofenceEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.GeofenceId, x.OccurredAtUtc });
            e.HasIndex(x => x.Position).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<TrackingAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.Severity, x.Status, x.OccurredAtUtc });
            e.HasIndex(x => x.Position).HasMethod("GIST"); // Spatial Index
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
        });

        modelBuilder.Entity<DeviceAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.Status });
        });

        // 3. Command & Integration Entities
        modelBuilder.Entity<DeviceCommand>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GpsDeviceId, x.Status });
            e.Property(x => x.CommandPayloadJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<DeviceCommandResult>(e =>
        {
            e.HasIndex(x => new { x.CommandId, x.Status });
            e.Property(x => x.ResultPayloadJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<GpsExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });

        modelBuilder.Entity<GpsIntegrationRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
        });

        modelBuilder.Entity<GpsAuditEntry>(e =>
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

        // 4. Telemetry Entities
        modelBuilder.Entity<OdometerReading>(e => e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.TimestampUtc }));
        modelBuilder.Entity<EngineStatus>(e => e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.TimestampUtc }));
        modelBuilder.Entity<FuelReading>(e => e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.TimestampUtc }));
        modelBuilder.Entity<SensorReading>(e => e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.TimestampUtc }));
    }
}

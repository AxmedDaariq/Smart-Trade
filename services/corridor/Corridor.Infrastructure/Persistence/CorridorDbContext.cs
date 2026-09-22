using Microsoft.EntityFrameworkCore;
using Corridor.Domain.Entities;

namespace Corridor.Infrastructure.Persistence;

public class CorridorDbContext : DbContext
{
    // 1. Corridor & Routes
    public DbSet<Domain.Entities.Corridor> Corridors { get; set; } = null!;
    public DbSet<CorridorVersion> CorridorVersions { get; set; } = null!;
    public DbSet<CorridorSegment> CorridorSegments { get; set; } = null!;
    public DbSet<CorridorSegmentRule> CorridorSegmentRules { get; set; } = null!;
    public DbSet<Route> Routes { get; set; } = null!;
    public DbSet<RouteVersion> RouteVersions { get; set; } = null!;
    public DbSet<RouteWaypoint> RouteWaypoints { get; set; } = null!;

    // 2. Checkpoints
    public DbSet<Checkpoint> Checkpoints { get; set; } = null!;
    public DbSet<RouteCheckpoint> RouteCheckpoints { get; set; } = null!;

    // 3. Trip Lifecycle & Events
    public DbSet<Trip> Trips { get; set; } = null!;
    public DbSet<TripVersion> TripVersions { get; set; } = null!;
    public DbSet<TripMilestone> TripMilestones { get; set; } = null!;
    public DbSet<TripCheckpointEvent> TripCheckpointEvents { get; set; } = null!;
    public DbSet<TripEvent> TripEvents { get; set; } = null!;
    public DbSet<TripStatusHistory> TripStatusHistories { get; set; } = null!;

    // 4. Eta & Delays
    public DbSet<TripEtaRecord> TripEtaRecords { get; set; } = null!;
    public DbSet<TripDelay> TripDelays { get; set; } = null!;
    public DbSet<RouteDeviation> RouteDeviations { get; set; } = null!;

    // 5. Incidents, Holds & Alerts
    public DbSet<TripIncident> TripIncidents { get; set; } = null!;
    public DbSet<CorridorAlert> CorridorAlerts { get; set; } = null!;
    public DbSet<TripHold> TripHolds { get; set; } = null!;

    // 6. Integrations & Platform Base
    public DbSet<CorridorExternalReference> CorridorExternalReferences { get; set; } = null!;
    public DbSet<CorridorIntegrationRecord> CorridorIntegrationRecords { get; set; } = null!;
    public DbSet<CorridorAuditEntry> CorridorAuditEntries { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;

    public CorridorDbContext(DbContextOptions<CorridorDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ensure PostGIS is enabled
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Domain.Entities.Corridor>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CorridorReference }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<CorridorVersion>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CorridorId, x.VersionNumber }).IsUnique();
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<CorridorSegment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CorridorVersionId, x.SequenceNumber }).IsUnique();
            e.Property(x => x.Geometry).HasColumnType("geometry(LineString,4326)");
            e.HasIndex(x => x.Geometry).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<CorridorSegmentRule>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CorridorSegmentId, x.Status });
        });

        modelBuilder.Entity<Route>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RouteReference }).IsUnique();
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<RouteVersion>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RouteId, x.VersionNumber }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.Geometry).HasColumnType("geometry(LineString,4326)");
            e.HasIndex(x => x.Geometry).HasMethod("GIST");
        });

        modelBuilder.Entity<RouteWaypoint>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RouteVersionId, x.SequenceNumber }).IsUnique();
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<Checkpoint>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CheckpointReference }).IsUnique();
            e.HasIndex(x => x.Status);
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
            e.Property(x => x.Geometry).HasColumnType("geometry(Geometry,4326)");
            e.HasIndex(x => x.Geometry).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<RouteCheckpoint>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RouteVersionId, x.SequenceNumber }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.CheckpointId });
        });

        modelBuilder.Entity<Trip>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.DispatchReference });
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.DriverReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.CorridorId, x.RouteId });
            e.Property(x => x.CurrentPosition).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.CurrentPosition).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<TripVersion>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.VersionNumber }).IsUnique();
        });

        modelBuilder.Entity<TripMilestone>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.SequenceNumber }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.TripId, x.Status, x.PlannedAtUtc });
            e.Property(x => x.Location).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Location).HasMethod("GIST");
        });

        modelBuilder.Entity<TripCheckpointEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.CheckpointId, x.OccurredAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<TripEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.EventType, x.OccurredAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<TripStatusHistory>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.ChangedAtUtc });
        });

        modelBuilder.Entity<TripEtaRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.CalculatedAtUtc });
        });

        modelBuilder.Entity<TripDelay>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.Status });
        });

        modelBuilder.Entity<RouteDeviation>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.Status, x.DetectedAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<TripIncident>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.Severity, x.Status, x.OccurredAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<CorridorAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.CorridorId, x.Severity, x.Status });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<TripHold>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.TripId, x.Status });
        });

        modelBuilder.Entity<CorridorExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });

        modelBuilder.Entity<CorridorIntegrationRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
        });

        modelBuilder.Entity<CorridorAuditEntry>(e =>
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

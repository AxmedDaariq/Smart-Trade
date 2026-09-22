using Microsoft.EntityFrameworkCore;
using AnprGate.Domain.Entities;

namespace AnprGate.Infrastructure.Persistence;

public class AnprDbContext : DbContext
{
    // 1. Camera
    public DbSet<AnprCamera> Cameras { get; set; } = null!;
    public DbSet<CameraConfiguration> CameraConfigurations { get; set; } = null!;
    public DbSet<CameraHealthRecord> CameraHealthRecords { get; set; } = null!;
    public DbSet<CameraCommand> CameraCommands { get; set; } = null!;

    // 2. Gate & Border
    public DbSet<Gate> Gates { get; set; } = null!;
    public DbSet<GateLane> GateLanes { get; set; } = null!;
    public DbSet<GateCameraAssignment> GateCameraAssignments { get; set; } = null!;
    public DbSet<BorderCheckpoint> BorderCheckpoints { get; set; } = null!;
    public DbSet<GateEvent> GateEvents { get; set; } = null!;

    // 3. Recognition & Observation
    public DbSet<AnprRecognitionEvent> RecognitionEvents { get; set; } = null!;
    public DbSet<PlateObservation> PlateObservations { get; set; } = null!;
    public DbSet<AnprVehicleMatch> VehicleMatches { get; set; } = null!;
    public DbSet<AnprTripMatch> TripMatches { get; set; } = null!;
    public DbSet<RecognitionEvidence> Evidences { get; set; } = null!;

    // 4. Authorization
    public DbSet<GateAuthorizationRequest> AuthorizationRequests { get; set; } = null!;
    public DbSet<GateAuthorizationDecision> AuthorizationDecisions { get; set; } = null!;

    // 5. Passage & Border Crossing
    public DbSet<GatePassage> GatePassages { get; set; } = null!;
    public DbSet<BorderCrossing> BorderCrossings { get; set; } = null!;
    public DbSet<BorderCrossingEvent> BorderCrossingEvents { get; set; } = null!;

    // 6. Operations & Alerts
    public DbSet<ManualVerification> ManualVerifications { get; set; } = null!;
    public DbSet<AnprManualOverride> ManualOverrides { get; set; } = null!;
    public DbSet<AnprAlert> Alerts { get; set; } = null!;

    // 7. Integrations & Platform Base
    public DbSet<AnprExternalReference> ExternalReferences { get; set; } = null!;
    public DbSet<AnprIntegrationRecord> IntegrationRecords { get; set; } = null!;
    public DbSet<AnprAuditEntry> AuditEntries { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;

    public AnprDbContext(DbContextOptions<AnprDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<AnprCamera>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CameraReference }).IsUnique();
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<CameraConfiguration>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CameraId, x.ConfigurationVersion }).IsUnique();
        });

        modelBuilder.Entity<CameraHealthRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CameraId, x.RecordedAtUtc });
        });

        modelBuilder.Entity<CameraCommand>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CameraId, x.Status });
        });

        modelBuilder.Entity<Gate>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GateReference }).IsUnique();
            e.Property(x => x.Location).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Location).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<GateLane>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GateId, x.LaneReference }).IsUnique();
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<GateCameraAssignment>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GateId, x.CameraId, x.LaneId });
        });

        modelBuilder.Entity<BorderCheckpoint>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.BorderReference }).IsUnique();
            e.Property(x => x.Location).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Location).HasMethod("GIST");
            e.Property(x => x.Version).IsRowVersion();
        });

        modelBuilder.Entity<GateEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.GateId, x.OccurredAtUtc });
        });

        modelBuilder.Entity<AnprRecognitionEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CameraId, x.ProviderEventId }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.NormalizedPlateText, x.RecognizedAtUtc });
            e.HasIndex(x => new { x.TenantId, x.Status });
        });

        modelBuilder.Entity<PlateObservation>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.NormalizedPlateText, x.ObservedAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<AnprVehicleMatch>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RecognitionEventId, x.VehicleReference });
        });

        modelBuilder.Entity<AnprTripMatch>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RecognitionEventId, x.TripReference });
        });

        modelBuilder.Entity<RecognitionEvidence>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RecognitionEventId });
        });

        modelBuilder.Entity<GateAuthorizationRequest>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.AuthorizationReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.VehicleReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.TripReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.GateId, x.Status, x.RequestedAtUtc });
        });

        modelBuilder.Entity<GateAuthorizationDecision>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.AuthorizationRequestId });
            e.Property(x => x.ConditionsJson).HasColumnType("jsonb");
        });

        modelBuilder.Entity<GatePassage>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.PassageReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.GateId, x.VehicleReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.GateId, x.TripReference, x.Status });
        });

        modelBuilder.Entity<BorderCrossing>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.CrossingReference }).IsUnique();
            e.HasIndex(x => new { x.TenantId, x.BorderCheckpointId, x.VehicleReference, x.Status });
            e.HasIndex(x => new { x.TenantId, x.BorderCheckpointId, x.TripReference, x.Status });
        });

        modelBuilder.Entity<BorderCrossingEvent>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.BorderCrossingId, x.OccurredAtUtc });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<ManualVerification>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.RecognitionEventId });
        });

        modelBuilder.Entity<AnprManualOverride>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
        });

        modelBuilder.Entity<AnprAlert>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.AlertType, x.Severity, x.Status });
            e.Property(x => x.Position).HasColumnType("geography(Point,4326)");
            e.HasIndex(x => x.Position).HasMethod("GIST");
        });

        modelBuilder.Entity<AnprExternalReference>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });
            e.HasIndex(x => new { x.ExternalSystem, x.ExternalReference });
        });

        modelBuilder.Entity<AnprIntegrationRecord>(e =>
        {
            e.HasIndex(x => new { x.TenantId, x.ProviderName, x.ProviderEventId }).IsUnique();
        });

        modelBuilder.Entity<AnprAuditEntry>(e =>
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

using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class TripCheckpointEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    public Guid CheckpointId { get; set; }
    public Guid? MilestoneId { get; set; }
    
    public string EventType { get; set; } = string.Empty; // Approached, Arrived, Entered, Exited, Completed, Skipped
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Source { get; set; } = string.Empty; // GPS, ANPR, Mobile, Manual, Integration, System
    public string? EvidenceReference { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
    public Checkpoint Checkpoint { get; set; } = null!;
    public TripMilestone? Milestone { get; set; }
}

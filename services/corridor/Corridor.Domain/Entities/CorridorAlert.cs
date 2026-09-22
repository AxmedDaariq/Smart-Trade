using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class CorridorAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid? TripId { get; set; }
    public Guid? CorridorId { get; set; }
    
    public string AlertType { get; set; } = string.Empty; // TripDelayed, RouteDeviation, CheckpointDelay, BorderDelay, UnexpectedStop, MissedCheckpoint, ETAChange, CorridorRestriction, Other
    public string Severity { get; set; } = string.Empty; // Info, Warning, High, Critical
    
    public DateTime OccurredAtUtc { get; set; }
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip? Trip { get; set; }
    public Corridor? Corridor { get; set; }
}

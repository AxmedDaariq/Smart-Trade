using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class TripMilestone
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string MilestoneReference { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    
    public string MilestoneType { get; set; } = string.Empty; // Origin, Dispatch, Port, Checkpoint, BorderApproach, BorderEntry, BorderExit, DryPort, Destination, Delivery, Custom
    
    public Point? Location { get; set; } // geography(Point,4326)
    
    public DateTime? PlannedAtUtc { get; set; }
    public DateTime? EstimatedAtUtc { get; set; }
    public DateTime? ActualAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Approaching, Arrived, Completed, Skipped, Delayed, Cancelled
    public string Source { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}

using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class AisAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    public Guid? PortCallId { get; set; }
    
    public string AlertType { get; set; } = string.Empty; // AISOffline, UnexpectedArrival, UnexpectedDeparture, LongAnchorage, RouteDeviation, RestrictedArea, SpeedAnomaly, PositionAnomaly, ETAChange, PortDelay, Other
    public string Severity { get; set; } = string.Empty; // Info, Warning, High, Critical
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}

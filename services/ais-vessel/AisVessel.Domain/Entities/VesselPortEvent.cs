using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class VesselPortEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortCallId { get; set; }
    public Guid VesselId { get; set; }
    
    public string EventType { get; set; } = string.Empty; // PortApproach, Anchored, Arrived, BerthingStarted, Berthed, CargoOperationsStarted, CargoOperationsCompleted, DepartureStarted, Departed, Other
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Source { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public PortCall PortCall { get; set; } = null!;
    public Vessel Vessel { get; set; } = null!;
}

using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class TripIncident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string IncidentReference { get; set; } = string.Empty;
    public string IncidentType { get; set; } = string.Empty; // Accident, Breakdown, Delay, Security, Cargo, RoadCondition, Weather, Medical, Documentation, Other
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public DateTime OccurredAtUtc { get; set; }
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Description { get; set; } = string.Empty;
    public Guid? ReportedBy { get; set; }
    
    public string Status { get; set; } = "Open"; // Open, Investigating, Resolved, Closed
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}

using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class RouteDeviation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    public Guid? RouteVersionId { get; set; }
    
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    
    public double DistanceFromPlannedRouteMeters { get; set; }
    public double? DeviationDurationMinutes { get; set; }
    
    public string Severity { get; set; } = "Warning"; // Warning, High, Critical
    public string DetectionSource { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    public string? Reason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}

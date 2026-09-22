using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class RouteVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RouteId { get; set; }
    public int VersionNumber { get; set; }
    
    public Guid? CorridorId { get; set; }
    public Guid? CorridorVersionId { get; set; }
    
    public LineString Geometry { get; set; } = null!; // LINESTRING 4326
    
    public double DistanceKm { get; set; }
    public double EstimatedDurationMinutes { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    public Guid CreatedBy { get; set; }
    public Guid? PublishedBy { get; set; }

    public Route Route { get; set; } = null!;
    public ICollection<RouteWaypoint> Waypoints { get; set; } = new List<RouteWaypoint>();
    public ICollection<RouteCheckpoint> Checkpoints { get; set; } = new List<RouteCheckpoint>();
}

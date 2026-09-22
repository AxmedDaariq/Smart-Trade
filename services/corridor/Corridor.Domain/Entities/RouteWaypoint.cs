using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class RouteWaypoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RouteVersionId { get; set; }
    
    public int SequenceNumber { get; set; }
    public string WaypointReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    
    public string WaypointType { get; set; } = string.Empty; // Origin, Destination, Checkpoint, Border, Port, DryPort, Warehouse, Custom
    
    public double? ExpectedArrivalOffsetMinutes { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public RouteVersion RouteVersion { get; set; } = null!;
}

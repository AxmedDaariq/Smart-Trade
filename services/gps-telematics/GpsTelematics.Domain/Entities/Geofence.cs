using NetTopologySuite.Geometries;

namespace GpsTelematics.Domain.Entities;

public class Geofence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string GeofenceReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GeofenceType { get; set; } = string.Empty; // Circle, Polygon, Corridor, Port, Border, DryPort, Checkpoint, Warehouse, RestrictedArea, Custom
    
    public Geometry Geometry { get; set; } = null!; // PostGIS geometry
    public Point? CenterPoint { get; set; }
    public double? RadiusMeters { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<GeofenceRule> Rules { get; set; } = new List<GeofenceRule>();
}

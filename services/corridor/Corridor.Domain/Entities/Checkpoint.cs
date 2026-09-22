using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class Checkpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string CheckpointReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CheckpointType { get; set; } = string.Empty; // Operational, Border, Port, DryPort, Inspection, RestArea, Fuel, Custom
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    public Geometry? Geometry { get; set; } // geometry(Geometry,4326)
    
    public string? CountryCode { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Closed, Restricted
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

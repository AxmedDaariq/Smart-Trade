using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class Port
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string PortReference { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    
    public string PortType { get; set; } = string.Empty; // Seaport, DryPort, Terminal, Anchorage, Other
    
    public Point Location { get; set; } = null!; // geography(Point,4326)
    public Geometry? BoundaryGeometry { get; set; } // geometry(Geometry,4326)
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

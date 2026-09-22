using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class PortFacility
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortId { get; set; }
    
    public string FacilityReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty; // Terminal, BerthArea, Warehouse, Gate, Anchorage, ContainerYard, Other
    
    public Point? Location { get; set; } // geography(Point,4326)
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Port Port { get; set; } = null!;
}

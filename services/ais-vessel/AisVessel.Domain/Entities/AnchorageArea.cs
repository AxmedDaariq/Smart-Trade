using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class AnchorageArea
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortId { get; set; }
    
    public string AnchorageReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public Geometry Geometry { get; set; } = null!; // geometry(Geometry,4326)
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Port Port { get; set; } = null!;
}

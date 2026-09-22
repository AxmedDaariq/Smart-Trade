using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class Berth
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortId { get; set; }
    public Guid? FacilityId { get; set; }
    
    public string BerthReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public Geometry? Geometry { get; set; } // geometry(Geometry,4326)
    
    public double? LengthMeters { get; set; }
    public double? DepthMeters { get; set; }
    
    public string Status { get; set; } = "Available"; // Available, Occupied, Reserved, Maintenance, Closed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Port Port { get; set; } = null!;
    public PortFacility? Facility { get; set; }
}

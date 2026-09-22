using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class BorderCheckpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string BorderReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string CountryFrom { get; set; } = string.Empty;
    public string CountryTo { get; set; } = string.Empty;
    
    public Point? Location { get; set; } // geography(Point,4326)
    
    public string Status { get; set; } = "Active"; // Active, Restricted, Closed, Inactive
    public string OperatingMode { get; set; } = "Normal"; // Normal, HighSecurity, Emergency, ReducedHours
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

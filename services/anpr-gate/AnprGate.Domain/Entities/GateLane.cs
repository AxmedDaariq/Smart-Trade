using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class GateLane
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GateId { get; set; }
    
    public string LaneReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound, Bidirectional
    public string LaneType { get; set; } = string.Empty; // Entry, Exit, Inspection, Mixed, Emergency, Other
    
    public string Status { get; set; } = "Open"; // Open, Closed, Restricted, Maintenance, Inactive
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Gate Gate { get; set; } = null!;
}

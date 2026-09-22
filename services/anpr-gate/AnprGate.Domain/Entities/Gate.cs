using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class Gate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string GateReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string GateType { get; set; } = string.Empty; // Port, Border, DryPort, Warehouse, Terminal, Checkpoint, Other
    
    public Point? Location { get; set; } // geography(Point,4326)
    
    public string Status { get; set; } = "Active"; // Active, Inactive, Maintenance, Decommissioned
    
    public string OperatingMode { get; set; } = "Normal"; // Normal, ManualOnly, FailClosed, FailOpenExplicitlyConfigured
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<GateLane> Lanes { get; set; } = new List<GateLane>();
    public ICollection<GateCameraAssignment> CameraAssignments { get; set; } = new List<GateCameraAssignment>();
}

using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class AnprCamera
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string CameraReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProviderReference { get; set; }
    
    public string CameraType { get; set; } = string.Empty; // ANPR, Overview, Entry, Exit, DualPurpose, Other
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound, Bidirectional, Unknown
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    
    public string Status { get; set; } = "Draft"; // Draft, Active, Suspended, Maintenance, Inactive, Retired
    public string OperationalStatus { get; set; } = "Unknown"; // Online, Degraded, Offline, Unknown
    
    public DateTime? InstalledAtUtc { get; set; }
    public DateTime? LastHeartbeatAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<CameraConfiguration> Configurations { get; set; } = new List<CameraConfiguration>();
    public ICollection<CameraHealthRecord> HealthRecords { get; set; } = new List<CameraHealthRecord>();
    public ICollection<CameraCommand> Commands { get; set; } = new List<CameraCommand>();
}

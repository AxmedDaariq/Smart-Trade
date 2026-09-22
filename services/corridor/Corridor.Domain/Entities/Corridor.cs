namespace Corridor.Domain.Entities;

public class Corridor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string CorridorReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public string OriginLocation { get; set; } = string.Empty;
    public string DestinationLocation { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Draft"; // Draft, Active, Suspended, Inactive, Archived
    
    public int CurrentVersion { get; set; } = 1;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<CorridorVersion> Versions { get; set; } = new List<CorridorVersion>();
}

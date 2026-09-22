namespace Corridor.Domain.Entities;

public class Route
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string RouteReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string OriginLocation { get; set; } = string.Empty;
    public string DestinationLocation { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Draft"; // Draft, Active, Suspended, Inactive, Archived
    
    public int CurrentVersion { get; set; } = 1;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<RouteVersion> Versions { get; set; } = new List<RouteVersion>();
}

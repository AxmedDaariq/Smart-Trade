namespace AisVessel.Domain.Entities;

public class VesselNavigationStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    
    public int StatusCode { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
    
    public DateTime TimestampUtc { get; set; }
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}

namespace AisVessel.Domain.Entities;

public class PortOperationalStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortId { get; set; }
    
    public string Status { get; set; } = "Normal"; // Normal, Congested, Restricted, Closed, Emergency, Unknown
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string? Reason { get; set; }
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Port Port { get; set; } = null!;
}

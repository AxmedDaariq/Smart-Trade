namespace AisVessel.Domain.Entities;

public class AisDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string DeviceReference { get; set; } = string.Empty;
    public string? MmsiNumber { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? FirmwareVersion { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Active, Inactive, Suspended, Retired
    
    public string? ProviderReference { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

namespace GpsTelematics.Domain.Entities;

public class TelematicsDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string ProviderReference { get; set; } = string.Empty;
    public string ExternalDeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    
    public string ConfigurationVersion { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    
    public DateTime InstalledAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RemovedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public GpsDevice GpsDevice { get; set; } = null!;
}

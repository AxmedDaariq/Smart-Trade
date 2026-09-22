namespace GpsTelematics.Domain.Entities;

public class DeviceHealth
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public double? BatteryPercent { get; set; }
    public double? SignalStrength { get; set; }
    public string GpsFixStatus { get; set; } = string.Empty;
    
    public DateTime? LastMessageAtUtc { get; set; }
    public string FirmwareVersion { get; set; } = string.Empty;
    
    public string HealthStatus { get; set; } = "Unknown"; // Healthy, Degraded, Offline, Fault, Unknown
    
    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

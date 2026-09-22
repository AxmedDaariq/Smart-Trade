namespace GpsTelematics.Domain.Entities;

public class GpsDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string DeviceReference { get; set; } = string.Empty;
    public string DeviceSerialNumber { get; set; } = string.Empty;
    public string? Imei { get; set; }
    
    public string DeviceType { get; set; } = string.Empty; // VehicleTracker, MobileDevice, IoTGateway, SensorGateway, Other
    
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string FirmwareVersion { get; set; } = string.Empty;
    
    public string Protocol { get; set; } = string.Empty; // HTTP, HTTPS, MQTT, TCP, UDP, Mobile, Other
    
    public string Status { get; set; } = "Pending"; // Pending, Active, Inactive, Suspended, Blocked, Retired
    
    public DateTime? LastConnectedAtUtc { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

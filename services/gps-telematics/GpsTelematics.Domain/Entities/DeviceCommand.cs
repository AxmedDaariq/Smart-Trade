namespace GpsTelematics.Domain.Entities;

public class DeviceCommand
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string CommandType { get; set; } = string.Empty; // RequestLocation, UpdateConfiguration, Restart, Diagnostics, Other
    
    public string CommandPayloadJson { get; set; } = "{}"; // JSONB
    
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending";
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

namespace GpsTelematics.Domain.Entities;

public class DeviceAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

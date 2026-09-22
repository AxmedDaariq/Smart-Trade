namespace GpsTelematics.Domain.Entities;

public class LocationStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public Guid GpsDeviceId { get; set; }
    
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Reason { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;

    public GpsDevice GpsDevice { get; set; } = null!;
}

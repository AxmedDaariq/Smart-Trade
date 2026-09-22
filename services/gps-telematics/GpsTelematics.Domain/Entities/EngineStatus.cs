namespace GpsTelematics.Domain.Entities;

public class EngineStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    public string VehicleReference { get; set; } = string.Empty;
    
    public string Status { get; set; } = string.Empty; // On, Off, Idle, Unknown
    
    public double? EngineHours { get; set; }
    public bool? IgnitionStatus { get; set; }
    
    public DateTime TimestampUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

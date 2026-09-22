namespace GpsTelematics.Domain.Entities;

public class SensorReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    public string VehicleReference { get; set; } = string.Empty;
    
    public string SensorType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    
    public DateTime TimestampUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Quality { get; set; } = string.Empty; // Good, Bad, Unknown
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

namespace GpsTelematics.Domain.Entities;

public class OdometerReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    public string VehicleReference { get; set; } = string.Empty;
    
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty; // Km, Miles
    
    public DateTime TimestampUtc { get; set; }
    
    public string Source { get; set; } = string.Empty;
    public string ValidationStatus { get; set; } = "Accepted"; // Accepted, Rejected, Quarantined
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

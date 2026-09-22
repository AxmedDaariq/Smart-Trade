namespace GpsTelematics.Domain.Entities;

public class FuelReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    public string VehicleReference { get; set; } = string.Empty;
    
    public double FuelLevel { get; set; }
    public string FuelUnit { get; set; } = string.Empty; // Liters, Gallons
    public double? FuelPercentage { get; set; }
    
    public DateTime TimestampUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Source { get; set; } = string.Empty;
    public string ValidationStatus { get; set; } = "Accepted"; // Accepted, Rejected, Quarantined
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}

namespace Fleet.Domain.Entities;

public class DriverCapability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DriverId { get; set; }
    
    public string CapabilityType { get; set; } = string.Empty; // CrossBorder, Hazmat, Container, HeavyVehicle, Refrigerated, Other
    
    public string Status { get; set; } = "Active";
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
}

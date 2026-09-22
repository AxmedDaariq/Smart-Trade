namespace Fleet.Domain.Entities;

public class VehicleCapability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? VehicleId { get; set; }
    public Guid? TrailerId { get; set; }
    
    public string CapabilityType { get; set; } = string.Empty; // ContainerTransport, Refrigerated, Hazmat, HeavyCargo, GPSCapable, CrossBorder, Other
    
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vehicle? Vehicle { get; set; }
    public Trailer? Trailer { get; set; }
}

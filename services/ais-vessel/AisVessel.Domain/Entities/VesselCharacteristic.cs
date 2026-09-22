namespace AisVessel.Domain.Entities;

public class VesselCharacteristic
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    
    public string CharacteristicType { get; set; } = string.Empty; // Length, Beam, Draft, GrossTonnage, NetTonnage, Deadweight, Capacity
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}

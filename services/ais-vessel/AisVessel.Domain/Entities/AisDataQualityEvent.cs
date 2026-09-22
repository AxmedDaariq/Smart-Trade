namespace AisVessel.Domain.Entities;

public class AisDataQualityEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid? VesselId { get; set; }
    public Guid? AisDeviceId { get; set; }
    
    public string? ProviderReference { get; set; }
    public string? EventId { get; set; }
    
    public string QualityType { get; set; } = string.Empty; // Duplicate, OutOfOrder, InvalidPosition, InvalidTimestamp, UnknownVessel, UnknownDevice, MissingIdentifier, MalformedPayload, ImpossibleSpeed, Other
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Description { get; set; } = string.Empty;
    public string? PayloadHash { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel? Vessel { get; set; }
    public AisDevice? AisDevice { get; set; }
}

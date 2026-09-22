namespace AisVessel.Domain.Entities;

public class VesselIdentifier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    
    public string IdentifierType { get; set; } = string.Empty; // IMO, MMSI, CallSign, NationalRegistration, ProviderReference, Other
    public string IdentifierValue { get; set; } = string.Empty;
    public string NormalizedValue { get; set; } = string.Empty;
    
    public string? IssuingAuthority { get; set; }
    
    public DateTime? ValidFromUtc { get; set; }
    public DateTime? ValidToUtc { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}

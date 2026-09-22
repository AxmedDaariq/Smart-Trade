namespace AisVessel.Domain.Entities;

public class VesselEtaRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    public Guid? VoyageId { get; set; }
    public Guid? PortCallId { get; set; }
    
    public DateTime EstimatedArrivalAtUtc { get; set; }
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Source { get; set; } = string.Empty; // AisProvider, PortSystem, CalculationEngine, Manual
    public string? ProviderReference { get; set; }
    
    public double? Confidence { get; set; }
    public string? Reason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}

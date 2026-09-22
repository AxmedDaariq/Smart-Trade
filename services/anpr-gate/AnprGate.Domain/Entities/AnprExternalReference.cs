namespace AnprGate.Domain.Entities;

public class AnprExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Camera, Gate, Passage, Border
    public Guid ResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // Fleet, GPS, Corridor, Customs, Payment, ECTN, AIS
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string? ExternalStatus { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

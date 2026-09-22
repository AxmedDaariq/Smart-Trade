namespace AisVessel.Domain.Entities;

public class AisExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Vessel, Port, Voyage, PortCall
    public Guid ResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // PortAuthority, AisProvider, Government
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string? ExternalStatus { get; set; }
    
    public DateTime? LastSynchronizedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

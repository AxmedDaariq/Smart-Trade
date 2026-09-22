namespace Fleet.Domain.Entities;

public class FleetExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Vehicle, Driver, Trailer, Dispatch
    public Guid ResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // GPS provider, ANPR provider, Customs, ECTN, Declaration, Corridor, Government, Other
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string ExternalStatus { get; set; } = string.Empty;
    
    public DateTime? LastSynchronizedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

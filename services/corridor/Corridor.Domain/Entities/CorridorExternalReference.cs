namespace Corridor.Domain.Entities;

public class CorridorExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Trip, Corridor, Route, Checkpoint
    public Guid ResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // Fleet, GPS, ANPR, Customs, ECTN, AIS, Payment
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string? ExternalStatus { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

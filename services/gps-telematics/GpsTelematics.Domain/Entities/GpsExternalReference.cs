namespace GpsTelematics.Domain.Entities;

public class GpsExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Device, Assignment, TrackingSession
    public Guid ResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // Fleet, ANPR, Corridor, GPS Provider
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string ExternalStatus { get; set; } = string.Empty;
    
    public DateTime? LastSynchronizedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

namespace Audit.Domain.Entities;

public class AuditAccessEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string? UserReference { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid? ResourceId { get; set; }
    
    public string? PermissionRequired { get; set; }
    public string PermissionResult { get; set; } = string.Empty;
    
    public string AccessMethod { get; set; } = string.Empty; // Web, Mobile, API, Service, Device, Integration
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

namespace Audit.Domain.Entities;

public class AuditResourceReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string? ResourceReference { get; set; }
    
    public string SourceService { get; set; } = string.Empty;
    public string? RelationshipType { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

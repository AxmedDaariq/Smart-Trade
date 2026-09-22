namespace Audit.Domain.Entities;

public class AuditChange
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string ChangeType { get; set; } = string.Empty; // Created, Updated, Deleted, Restored, StatusChanged, ConfigurationChanged
    
    public string? BeforeSnapshotJson { get; set; } // JSONB (Sanitized)
    public string? AfterSnapshotJson { get; set; } // JSONB (Sanitized)
    public string? ChangedFieldsJson { get; set; } // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

namespace Audit.Domain.Entities;

public class AuditDataAccess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid? ResourceId { get; set; }
    
    public string AccessType { get; set; } = string.Empty; // Read, Write, Export, Download, Search, Other
    
    public string? Purpose { get; set; }
    public string Result { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

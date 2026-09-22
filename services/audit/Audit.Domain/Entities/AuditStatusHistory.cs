namespace Audit.Domain.Entities;

public class AuditStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string? PreviousStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

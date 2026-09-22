namespace Audit.Domain.Entities;

public class AuditApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string ApprovalType { get; set; } = string.Empty;
    
    public Guid SubmittedBy { get; set; }
    public Guid? ReviewedBy { get; set; }
    
    public string Decision { get; set; } = string.Empty; // Approved, Rejected, Returned, Cancelled
    public string? Reason { get; set; }
    
    public DateTime SubmittedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

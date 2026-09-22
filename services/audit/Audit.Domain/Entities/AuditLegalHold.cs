namespace Audit.Domain.Entities;

public class AuditLegalHold
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string Reference { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    
    public DateTime StartAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Released, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

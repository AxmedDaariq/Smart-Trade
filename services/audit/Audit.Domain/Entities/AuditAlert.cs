namespace Audit.Domain.Entities;

public class AuditAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string AlertReference { get; set; } = string.Empty;
    
    public string AlertType { get; set; } = string.Empty; // CrossTenantAttempt, PrivilegeEscalation, BruteForce, AuditTampering, MassExport, SuspiciousAccess...
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public Guid? SourceAuditEventId { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Investigating, Resolved, Acknowledged, FalsePositive
    
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

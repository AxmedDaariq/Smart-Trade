namespace Audit.Domain.Entities;

public class AuditSecurityEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string SecurityEventType { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public string DetectionSource { get; set; } = string.Empty;
    
    public string? IpAddress { get; set; }
    public string? DeviceReference { get; set; }
    public string? UserReference { get; set; }
    public string? ResourceReference { get; set; }
    
    public string DetectionReason { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Investigating, Resolved, FalsePositive
    
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime? ResolvedAtUtc { get; set; }
    public Guid? ResolvedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

namespace Audit.Domain.Entities;

public class AuditIntegrityRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string? PartitionReference { get; set; }
    
    public Guid FirstAuditEventId { get; set; }
    public Guid LastAuditEventId { get; set; }
    
    public long EventCount { get; set; }
    
    public string FirstHash { get; set; } = string.Empty;
    public string LastHash { get; set; } = string.Empty;
    
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string VerificationStatus { get; set; } = "NotVerified"; // Valid, Invalid, NotVerified, Partial
    public string? VerificationError { get; set; }
}

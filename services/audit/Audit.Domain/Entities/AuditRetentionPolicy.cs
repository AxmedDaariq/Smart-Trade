namespace Audit.Domain.Entities;

public class AuditRetentionPolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public int MinimumRetentionDays { get; set; }
    public int? ArchiveAfterDays { get; set; }
    public int? DeleteAfterDays { get; set; }
    
    public bool LegalHoldEnabled { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Inactive, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

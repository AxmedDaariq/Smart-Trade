namespace Audit.Domain.Entities;

public class AuditConfigurationChange
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string ConfigurationType { get; set; } = string.Empty;
    public string ConfigurationReference { get; set; } = string.Empty;
    
    public string? VersionNumber { get; set; }
    public string? PreviousVersionNumber { get; set; }
    
    public string ChangeType { get; set; } = string.Empty;
    
    public string? LegalBasis { get; set; }
    public string? Reason { get; set; }
    
    public DateTime? EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public Guid ChangedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}

namespace Notification.Domain.Entities;

public class NotificationTemplateVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid TemplateId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public string? SubjectTemplate { get; set; }
    public string BodyTemplate { get; set; } = string.Empty;
    
    public string VariablesSchemaJson { get; set; } = "{}"; // JSONB
    
    public string LanguageCode { get; set; } = "en";
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Published, Superseded, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }

    public NotificationTemplate Template { get; set; } = null!;
}

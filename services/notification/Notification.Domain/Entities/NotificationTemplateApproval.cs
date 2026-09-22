namespace Notification.Domain.Entities;

public class NotificationTemplateApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid TemplateVersionId { get; set; }
    
    public Guid SubmittedBy { get; set; }
    public Guid? ReviewedBy { get; set; }
    
    public string Decision { get; set; } = "Pending"; // Approved, Rejected, Returned, Pending
    public string? Reason { get; set; }
    
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }

    public NotificationTemplateVersion TemplateVersion { get; set; } = null!;
}

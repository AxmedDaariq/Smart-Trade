namespace Notification.Domain.Entities;

public class NotificationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string RequestReference { get; set; } = string.Empty;
    
    public string SourceService { get; set; } = string.Empty;
    public string SourceEventType { get; set; } = string.Empty;
    public string? SourceEventId { get; set; }
    
    public string NotificationType { get; set; } = string.Empty;
    
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? RequestedBy { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Received"; // Received, Validated, Queued, Processing, Completed, Failed, Rejected
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

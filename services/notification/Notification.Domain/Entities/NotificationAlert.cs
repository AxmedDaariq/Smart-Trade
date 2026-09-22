namespace Notification.Domain.Entities;

public class NotificationAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string AlertReference { get; set; } = string.Empty;
    
    public string AlertType { get; set; } = string.Empty; // ProviderOffline, HighFailureRate, HighLatency, DeliveryBacklog, DLQGrowth, AuthenticationFailure, QuotaExceeded, TemplateFailure, ProviderCallbackFailure, Other
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public string? ChannelType { get; set; }
    public string? ProviderReference { get; set; }
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Description { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

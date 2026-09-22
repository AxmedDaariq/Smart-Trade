namespace Notification.Domain.Entities;

public class NotificationDeliveryAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid DeliveryId { get; set; }
    
    public int AttemptNumber { get; set; }
    
    public DateTime AttemptedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    
    public string Status { get; set; } = "Started"; // Started, Succeeded, Failed, TimedOut, Cancelled
    
    public string? ProviderStatusCode { get; set; }
    public string? ProviderMessageReference { get; set; }
    
    public long? LatencyMs { get; set; }
    
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    public string? ResponseHash { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public NotificationDelivery Delivery { get; set; } = null!;
}

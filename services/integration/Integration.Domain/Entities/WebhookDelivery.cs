namespace Integration.Domain.Entities;

public class WebhookDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid WebhookSubscriptionId { get; set; }
    public Guid EventId { get; set; }
    
    public int AttemptNumber { get; set; }
    
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResponseAtUtc { get; set; }
    
    public int? HttpStatusCode { get; set; }
    public long? DurationMs { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Sending, Succeeded, Failed, DeadLettered, Cancelled
    
    public string? ResponseHash { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTime? NextRetryAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public WebhookSubscription Subscription { get; set; } = null!;
}

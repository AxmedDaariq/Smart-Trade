namespace Notification.Domain.Entities;

public class NotificationProviderCallback
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderEventId { get; set; }
    public string? ProviderMessageReference { get; set; }
    
    public string ChannelType { get; set; } = string.Empty;
    
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? OccurredAtUtc { get; set; }
    
    public bool SignatureVerified { get; set; }
    public string PayloadHash { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Received"; // Received, Validated, Processed, Rejected, Duplicate, Quarantined
    
    public DateTime? ProcessedAtUtc { get; set; }
    public string? ErrorCode { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

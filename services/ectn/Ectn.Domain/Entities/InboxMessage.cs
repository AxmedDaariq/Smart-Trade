namespace Ectn.Domain.Entities;

public class InboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    
    public string MessageType { get; set; } = string.Empty;
    public string Consumer { get; set; } = string.Empty;
    
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending";
    public int RetryCount { get; set; }
    public string LastError { get; set; } = string.Empty;
    public string PayloadHash { get; set; } = string.Empty;
}

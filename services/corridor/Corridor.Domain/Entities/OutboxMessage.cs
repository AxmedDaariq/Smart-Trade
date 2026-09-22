namespace Corridor.Domain.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MessageId { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public Guid TenantId { get; set; }
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public string CorrelationId { get; set; } = string.Empty;
    public string CausationId { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = "1.0";
    
    public string PayloadJson { get; set; } = string.Empty; // JSONB
    
    public string Status { get; set; } = "Pending"; // Pending, Published, Failed, DeadLetter
    
    public DateTime? PublishedAtUtc { get; set; }
    
    public int RetryCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

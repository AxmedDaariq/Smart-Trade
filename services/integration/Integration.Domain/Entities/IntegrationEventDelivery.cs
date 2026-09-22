namespace Integration.Domain.Entities;

public class IntegrationEventDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid MessageId { get; set; }
    public Guid ContractId { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    
    public string SourceSystem { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAtUtc { get; set; }
    
    public string Status { get; set; } = "Received"; // Received, Queued, Processing, Delivered, Failed, DeadLettered, Cancelled
    
    public int AttemptCount { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    public string? CausationId { get; set; }
    
    public string PayloadHash { get; set; } = string.Empty;
    public string? LastError { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationEventContract Contract { get; set; } = null!;
}

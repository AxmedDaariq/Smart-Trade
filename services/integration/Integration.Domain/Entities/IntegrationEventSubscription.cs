namespace Integration.Domain.Entities;

public class IntegrationEventSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid ContractId { get; set; }
    
    public string SubscriberSystem { get; set; } = string.Empty;
    public string ConsumerName { get; set; } = string.Empty;
    
    public string? RoutingKey { get; set; }
    public string? Exchange { get; set; }
    public string? QueueName { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Paused, Suspended, Retired
    
    public string? RetryPolicyReference { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationEventContract Contract { get; set; } = null!;
}

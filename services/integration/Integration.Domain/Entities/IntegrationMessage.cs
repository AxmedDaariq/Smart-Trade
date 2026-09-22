namespace Integration.Domain.Entities;

public class IntegrationMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid MessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    
    public string SourceSystem { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    
    public Guid? EndpointId { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    public string? CausationId { get; set; }
    
    public string PayloadHash { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    
    public string Status { get; set; } = "Received"; // Received, Validated, Processing, Processed, Failed, Skipped, DeadLettered
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public IntegrationEndpoint? Endpoint { get; set; }
}

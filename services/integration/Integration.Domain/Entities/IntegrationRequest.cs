namespace Integration.Domain.Entities;

public class IntegrationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid IntegrationSystemId { get; set; }
    public Guid EndpointId { get; set; }
    
    public string RequestReference { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string? IdempotencyKey { get; set; }
    
    public string? HttpMethod { get; set; }
    public string RequestPath { get; set; } = string.Empty;
    
    public DateTime RequestTimestampUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResponseTimestampUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Succeeded, Failed, Timeout, Cancelled
    public int? HttpStatusCode { get; set; }
    public int AttemptCount { get; set; }
    public long? DurationMs { get; set; }
    
    public string? RequestPayloadHash { get; set; }
    public string? ResponsePayloadHash { get; set; }
    
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
    public IntegrationEndpoint Endpoint { get; set; } = null!;
}

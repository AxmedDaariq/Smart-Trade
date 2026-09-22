namespace Integration.Domain.Entities;

public class IntegrationRateLimitPolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    public Guid? EndpointId { get; set; }
    
    public int? RequestsPerSecond { get; set; }
    public int? RequestsPerMinute { get; set; }
    public int? BurstLimit { get; set; }
    
    public string Strategy { get; set; } = "FixedWindow"; // FixedWindow, SlidingWindow, TokenBucket
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
    public IntegrationEndpoint? Endpoint { get; set; }
}

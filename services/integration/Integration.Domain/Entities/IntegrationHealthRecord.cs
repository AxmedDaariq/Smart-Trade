namespace Integration.Domain.Entities;

public class IntegrationHealthRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    public Guid? EndpointId { get; set; }
    
    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Unknown"; // Healthy, Degraded, Unhealthy, Unknown
    
    public int? LatencyMs { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ErrorCode { get; set; }
    
    public double? AvailabilityScore { get; set; }
    
    public string? DetailsJson { get; set; } // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
    public IntegrationEndpoint? Endpoint { get; set; }
}

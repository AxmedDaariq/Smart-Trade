namespace Integration.Domain.Entities;

public class IntegrationCircuitState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    public Guid? EndpointId { get; set; }
    
    public string State { get; set; } = "Closed"; // Closed, Open, HalfOpen
    
    public int FailureCount { get; set; }
    public int SuccessCount { get; set; }
    
    public DateTime? OpenedAtUtc { get; set; }
    public DateTime? HalfOpenAtUtc { get; set; }
    
    public DateTime? LastFailureAtUtc { get; set; }
    public DateTime? LastSuccessAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
    public IntegrationEndpoint? Endpoint { get; set; }
}

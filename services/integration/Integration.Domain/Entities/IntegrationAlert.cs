namespace Integration.Domain.Entities;

public class IntegrationAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string AlertReference { get; set; } = string.Empty;
    public Guid? SystemId { get; set; }
    public Guid? EndpointId { get; set; }
    
    public string AlertType { get; set; } = string.Empty; // ProviderOffline, HighFailureRate, HighLatency, AuthenticationFailure, RateLimitExceeded, WebhookFailure, EventBacklog, DeadLetterGrowth, SchemaMismatch, SignatureFailure, CertificateExpiry, IntegrationTimeout, RepeatedRetry, Other
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Description { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem? System { get; set; }
    public IntegrationEndpoint? Endpoint { get; set; }
}

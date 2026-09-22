namespace GpsTelematics.Domain.Entities;

public class GpsIntegrationRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string IntegrationName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    public string Operation { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending";
    
    public string ExternalReference { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

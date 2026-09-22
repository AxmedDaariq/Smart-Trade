namespace Declaration.Domain.Entities;

public class DeclarationIntegrationRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid DeclarationId { get; set; }
    
    public string IntegrationName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    public string Operation { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Processing, Succeeded, Failed, DeadLetter, Cancelled
    
    public string ExternalReference { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

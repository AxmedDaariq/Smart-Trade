namespace Integration.Domain.Entities;

public class IntegrationSchemaValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid? ContractId { get; set; }
    public Guid MessageId { get; set; }
    
    public string ValidationDirection { get; set; } = string.Empty; // Inbound, Outbound
    public string SchemaVersion { get; set; } = string.Empty;
    
    public bool IsValid { get; set; }
    public string? ValidationErrorsJson { get; set; } // JSONB
    
    public DateTime ValidatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationEventContract? Contract { get; set; }
}

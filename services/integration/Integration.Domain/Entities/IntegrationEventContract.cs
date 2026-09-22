namespace Integration.Domain.Entities;

public class IntegrationEventContract
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ContractReference { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    
    public string SourceSystem { get; set; } = string.Empty;
    public string? TargetSystem { get; set; }
    
    public string SchemaJson { get; set; } = string.Empty; // JSONB
    
    public string Compatibility { get; set; } = "BackwardCompatible"; // BackwardCompatible, Strict, BreakingChange
    
    public string Status { get; set; } = "Active"; // Draft, Active, Deprecated, Retired
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

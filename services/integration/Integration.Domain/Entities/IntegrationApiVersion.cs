namespace Integration.Domain.Entities;

public class IntegrationApiVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    
    public string Version { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Draft, Active, Deprecated, Retired
    
    public string CompatibilityMode { get; set; } = "Strict"; // Strict, BackwardCompatible, TransformRequired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem System { get; set; } = null!;
}

namespace Integration.Domain.Entities;

public class IntegrationTransformationRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid MessageId { get; set; }
    public Guid MappingId { get; set; }
    
    public string SourceSchemaVersion { get; set; } = string.Empty;
    public string TargetSchemaVersion { get; set; } = string.Empty;
    
    public string TransformationStatus { get; set; } = "Pending"; // Pending, Succeeded, Failed, Skipped
    
    public string InputHash { get; set; } = string.Empty;
    public string OutputHash { get; set; } = string.Empty;
    
    public DateTime TransformedAtUtc { get; set; } = DateTime.UtcNow;
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationMapping Mapping { get; set; } = null!;
}

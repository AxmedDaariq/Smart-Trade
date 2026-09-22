namespace Integration.Domain.Entities;

public class IntegrationMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    
    public string MappingReference { get; set; } = string.Empty;
    
    public string SourceSchema { get; set; } = string.Empty;
    public string TargetSchema { get; set; } = string.Empty;
    
    public string SourceVersion { get; set; } = string.Empty;
    public string TargetVersion { get; set; } = string.Empty;
    
    public string MappingType { get; set; } = string.Empty; // Direct, Rename, Transform, Conditional, Lookup, Composite, CustomAdapter
    
    public string ConfigurationJson { get; set; } = "{}"; // JSONB - Declarative only! No executable code.
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public IntegrationSystem System { get; set; } = null!;
}

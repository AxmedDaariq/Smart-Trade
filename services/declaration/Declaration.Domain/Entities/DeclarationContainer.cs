namespace Declaration.Domain.Entities;

public class DeclarationContainer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    
    public string ContainerNumber { get; set; } = string.Empty;
    public string NormalizedContainerNumber { get; set; } = string.Empty;
    public string ContainerType { get; set; } = string.Empty;
    public string SealNumber { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    public int PackageCount { get; set; }
    
    public string Status { get; set; } = "Declared"; // Declared, Verified, Loaded, InTransit, Discharged, Released, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
}

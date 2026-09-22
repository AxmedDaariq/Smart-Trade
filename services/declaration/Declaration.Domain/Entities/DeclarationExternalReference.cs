namespace Declaration.Domain.Entities;

public class DeclarationExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // ECTN, Port, Government, ExternalCustoms, Partner, Trade system
    public string ReferenceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public string ExternalStatus { get; set; } = string.Empty;
    
    public DateTime LastSynchronizedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

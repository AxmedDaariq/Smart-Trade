namespace Customs.Domain.Entities.Configuration;

public class ExemptionRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string RuleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string EligibilityDefinitionJson { get; set; } = "{}"; // JSONB
    
    public string? LegalBasis { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Published, Superseded, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

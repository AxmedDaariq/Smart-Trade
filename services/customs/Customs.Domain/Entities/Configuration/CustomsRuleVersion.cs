namespace Customs.Domain.Entities.Configuration;

public class CustomsRuleVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsRuleId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public string RuleDefinitionJson { get; set; } = "{}"; // JSONB
    
    public string? LegalBasis { get; set; }
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft";
    public string ChangeReason { get; set; } = string.Empty;
    
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public Guid? PublishedBy { get; set; }
    public DateTime? PublishedAtUtc { get; set; }

    public CustomsRule CustomsRule { get; set; } = null!;
}

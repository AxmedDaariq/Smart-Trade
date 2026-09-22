namespace Customs.Domain.Entities.Configuration;

public class RiskRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string RuleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public int Priority { get; set; }
    public string RuleDefinitionJson { get; set; } = "{}"; // JSONB
    
    public int Score { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Approved, Published, Superseded, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

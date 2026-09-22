using Customs.Domain.Entities.Configuration;

namespace Customs.Domain.Entities;

public class RiskRuleMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiskAssessmentId { get; set; }
    
    public Guid RiskRuleId { get; set; }
    public string RuleVersion { get; set; } = string.Empty;
    
    public bool Matched { get; set; }
    public int ScoreContribution { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    public string EvidenceJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public RiskAssessment RiskAssessment { get; set; } = null!;
    public RiskRule RiskRule { get; set; } = null!;
}

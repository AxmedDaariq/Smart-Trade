namespace Customs.Domain.Entities;

public class RiskAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    public Guid DeclarationId { get; set; }
    
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public string Decision { get; set; } = string.Empty; // NoInspection, DocumentReview, PhysicalInspection, SupervisorReview, Hold
    
    public string RulesVersion { get; set; } = string.Empty;
    
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CalculatedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
    public ICollection<RiskRuleMatch> RuleMatches { get; set; } = new List<RiskRuleMatch>();
}

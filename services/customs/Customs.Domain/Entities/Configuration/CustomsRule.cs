namespace Customs.Domain.Entities.Configuration;

public class CustomsRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string RuleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty; // TariffSelection, DutyCalculation, TaxCalculation, ProcedureEligibility, ExemptionEligibility, Risk, Clearance, Release, Validation, Other
    
    public string Status { get; set; } = "Draft";
    public int CurrentVersionNumber { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<CustomsRuleVersion> Versions { get; set; } = new List<CustomsRuleVersion>();
}

namespace Customs.Domain.Entities;

public class CustomsAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    public Guid DeclarationId { get; set; }
    
    public int DeclarationVersionNumber { get; set; }
    
    public string AssessmentNumber { get; set; } = string.Empty;
    public int AssessmentVersion { get; set; }
    
    public Guid TariffScheduleVersionId { get; set; }
    public Guid ValuationId { get; set; }
    public Guid? ExchangeRateConfigurationId { get; set; }
    
    public string RulesVersion { get; set; } = string.Empty;
    
    public decimal CustomsValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public decimal DutyAmount { get; set; }
    public decimal CustomsChargesAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Calculated, Reviewed, Approved, Superseded, Cancelled
    
    public DateTime? CalculatedAtUtc { get; set; }
    public Guid? CalculatedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    
    public ICollection<DutyAssessment> DutyAssessments { get; set; } = new List<DutyAssessment>();
    public ICollection<TaxAssessment> TaxAssessments { get; set; } = new List<TaxAssessment>();
    public ICollection<CustomsAssessmentLine> Lines { get; set; } = new List<CustomsAssessmentLine>();
}

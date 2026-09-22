namespace Customs.Domain.Entities;

public class CustomsHold
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public string HoldType { get; set; } = string.Empty; // DocumentHold, InspectionHold, PaymentHold, RiskHold, LegalHold, Other
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid PlacedBy { get; set; }
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ReleasedBy { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Released, Cancelled, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
}

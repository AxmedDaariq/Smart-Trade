namespace Customs.Domain.Entities;

public class ExemptionDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExemptionId { get; set; }
    
    public string Decision { get; set; } = string.Empty; // Approved, Rejected, PartiallyApproved
    
    public decimal ApprovedAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid DecidedBy { get; set; }
    public DateTime DecidedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Exemption Exemption { get; set; } = null!;
}

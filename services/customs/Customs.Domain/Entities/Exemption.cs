using Customs.Domain.Entities.Configuration;

namespace Customs.Domain.Entities;

public class Exemption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public Guid ExemptionRuleId { get; set; }
    
    public decimal RequestedAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Requested"; // Requested, UnderReview, Approved, Rejected, Cancelled, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    public ExemptionRule ExemptionRule { get; set; } = null!;
    public ICollection<ExemptionDecision> Decisions { get; set; } = new List<ExemptionDecision>();
}

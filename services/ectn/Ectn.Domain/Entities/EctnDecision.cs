namespace Ectn.Domain.Entities;

public class EctnDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public string DecisionType { get; set; } = string.Empty; // Approval, Rejection, AmendmentApproval, Cancellation, Reissue
    public string Decision { get; set; } = string.Empty; // Approved, Rejected
    
    public string DecisionReason { get; set; } = string.Empty;
    public string LegalBasis { get; set; } = string.Empty;
    
    public Guid DecidedBy { get; set; }
    public DateTime DecidedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

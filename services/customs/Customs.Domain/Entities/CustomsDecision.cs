namespace Customs.Domain.Entities;

public class CustomsDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public string DecisionType { get; set; } = string.Empty; // AssessmentApproval, ClearanceApproval, ClearanceRejection, ReleaseApproval, HoldDecision, ReassessmentDecision
    public string Decision { get; set; } = string.Empty; // Approved, Rejected, Returned, Held
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid DecidedBy { get; set; }
    public DateTime DecidedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
}

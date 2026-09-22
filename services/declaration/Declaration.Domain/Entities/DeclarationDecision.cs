namespace Declaration.Domain.Entities;

public class DeclarationDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    public Guid DeclarationVersionId { get; set; }
    
    public string DecisionType { get; set; } = string.Empty; // Acceptance, Rejection, ReturnForCorrection, Cancellation
    public string Decision { get; set; } = string.Empty; // Accepted, Rejected, Returned, Cancelled
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid DecidedBy { get; set; }
    public DateTime DecidedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

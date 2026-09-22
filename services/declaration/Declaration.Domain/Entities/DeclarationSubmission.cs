namespace Declaration.Domain.Entities;

public class DeclarationSubmission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    public Guid DeclarationVersionId { get; set; }
    
    public string SubmissionReference { get; set; } = string.Empty;
    public Guid SubmittedBy { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Channel { get; set; } = string.Empty; // Web, API, Mobile, Integration, Batch, Other
    public string Status { get; set; } = "Submitted"; // Submitted, Accepted, Rejected, Failed, Withdrawn
    
    public string RequestHash { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

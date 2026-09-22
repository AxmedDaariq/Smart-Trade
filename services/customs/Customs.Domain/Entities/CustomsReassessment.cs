namespace Customs.Domain.Entities;

public class CustomsReassessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public Guid OriginalAssessmentId { get; set; }
    public Guid? NewAssessmentId { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    
    public Guid TriggeredBy { get; set; }
    public DateTime TriggeredAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Requested"; // Requested, UnderReview, Completed, Rejected, Cancelled
    
    public DateTime? CompletedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
}

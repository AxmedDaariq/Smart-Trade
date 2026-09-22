namespace Customs.Domain.Entities;

public class CustomsClearance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    public Guid AssessmentId { get; set; }
    
    public string ClearanceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, UnderReview, Cleared, Rejected, Cancelled
    
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public Guid? RejectedBy { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    
    public string? Reason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    public CustomsAssessment Assessment { get; set; } = null!;
    public ICollection<ReleaseAuthorization> ReleaseAuthorizations { get; set; } = new List<ReleaseAuthorization>();
}

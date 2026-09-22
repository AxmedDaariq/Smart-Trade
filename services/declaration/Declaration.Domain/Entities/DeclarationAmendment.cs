namespace Declaration.Domain.Entities;

public class DeclarationAmendment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    
    public int PreviousVersionNumber { get; set; }
    public int RequestedVersionNumber { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public string Status { get; set; } = "Requested"; // Requested, UnderReview, Approved, Rejected, Applied, Cancelled
    
    public string ChangeSummaryJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

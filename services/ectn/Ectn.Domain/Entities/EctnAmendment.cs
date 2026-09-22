namespace Ectn.Domain.Entities;

public class EctnAmendment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public int AmendmentNumber { get; set; }
    public int PreviousVersion { get; set; }
    public int NewVersion { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public string Status { get; set; } = "Requested"; // Requested, UnderReview, Approved, Rejected, Applied
    
    public string ChangeSummaryJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

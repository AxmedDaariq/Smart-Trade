namespace Ectn.Domain.Entities;

public class EctnReview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public Guid ReviewerUserId { get; set; }
    
    public string ReviewType { get; set; } = string.Empty; // Initial, Amendment, Renewal, Reissue, Compliance
    public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Approved, Rejected, Returned
    
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    
    public string Comments { get; set; } = string.Empty;
    public string DecisionReason { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

namespace Customs.Domain.Entities;

public class InspectionOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public string InspectionReference { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty; // DocumentInspection, PhysicalInspection, ScannerInspection, JointInspection
    
    public string Priority { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    
    public Guid? AssignedOfficerId { get; set; }
    
    public DateTime? AssignedAtUtc { get; set; }
    public DateTime? ScheduledAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Assigned, InProgress, Completed, Cancelled, Escalated
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    public ICollection<InspectionResult> Results { get; set; } = new List<InspectionResult>();
}

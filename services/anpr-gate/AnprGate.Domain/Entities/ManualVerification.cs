namespace AnprGate.Domain.Entities;

public class ManualVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid? RecognitionEventId { get; set; }
    public Guid? AuthorizationRequestId { get; set; }
    public Guid? GatePassageId { get; set; }
    public Guid? BorderCrossingId { get; set; }
    
    public Guid OfficerUserId { get; set; }
    
    public string VerificationType { get; set; } = string.Empty; // Plate, Vehicle, Driver, Trip, Document, Authorization, Border
    
    public string Decision { get; set; } = string.Empty; // Confirmed, Rejected, Escalated
    public string Reason { get; set; } = string.Empty;
    
    public string? EvidenceReference { get; set; }
    
    public DateTime VerifiedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

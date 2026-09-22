namespace AnprGate.Domain.Entities;

public class RecognitionEvidence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RecognitionEventId { get; set; }
    
    public string ObjectStorageKey { get; set; } = string.Empty;
    public string EvidenceType { get; set; } = string.Empty; // PlateImage, VehicleImage, Frame, VideoReference, Other
    
    public string ContentHashSha256 { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    public DateTime CapturedAtUtc { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string MalwareScanStatus { get; set; } = "Pending"; // Pending, Clean, Infected, Failed, NotRequired
    public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected, Quarantined
    
    public DateTime? RetentionUntilUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprRecognitionEvent RecognitionEvent { get; set; } = null!;
}

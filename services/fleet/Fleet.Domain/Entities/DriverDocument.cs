namespace Fleet.Domain.Entities;

public class DriverDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid DriverId { get; set; }
    
    public string DocumentType { get; set; } = string.Empty; // License, ID, Qualification, TrainingCertificate, MedicalCertificate, Employment, Permit, Other
    public string DocumentNumber { get; set; } = string.Empty;
    
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    
    public string StorageProvider { get; set; } = string.Empty;
    public string StorageObjectKey { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Uploaded"; // Uploaded, PendingVerification, Verified, Rejected, Quarantined, Expired, Deleted
    
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
}

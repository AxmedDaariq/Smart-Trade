namespace Ectn.Domain.Entities;

public class EctnDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid EctnApplicationId { get; set; }
    
    public string DocumentType { get; set; } = string.Empty; // CommercialInvoice, BillOfLading, PackingList, etc.
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    
    public string StorageProvider { get; set; } = string.Empty;
    public string StorageObjectKey { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Uploaded"; // Uploaded, PendingVerification, Verified, Rejected, Quarantined, Deleted
    
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public EctnApplication EctnApplication { get; set; } = null!;
    public ICollection<EctnDocumentVersion> DocumentVersions { get; set; } = new List<EctnDocumentVersion>();
    public ICollection<EctnDocumentVerification> Verifications { get; set; } = new List<EctnDocumentVerification>();
}

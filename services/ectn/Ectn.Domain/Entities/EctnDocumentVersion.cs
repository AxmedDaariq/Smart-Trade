namespace Ectn.Domain.Entities;

public class EctnDocumentVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnDocumentId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public string StorageObjectKey { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Active";
    
    public EctnDocument EctnDocument { get; set; } = null!;
}

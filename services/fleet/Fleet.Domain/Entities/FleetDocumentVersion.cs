namespace Fleet.Domain.Entities;

public class FleetDocumentVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string DocumentType { get; set; } = string.Empty; // VehicleDocument, DriverDocument
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    
    public string StorageObjectKey { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

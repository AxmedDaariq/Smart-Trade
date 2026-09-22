namespace Audit.Domain.Entities;

public class AuditArchive
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ArchiveReference { get; set; } = string.Empty;
    
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    
    public long RecordCount { get; set; }
    
    public string StorageReference { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    
    public string EncryptionStatus { get; set; } = "Encrypted"; // Encrypted, Plaintext
    public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Corrupt
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ArchivedAtUtc { get; set; }
}

namespace Integration.Domain.Entities;

public class IntegrationFileExchange
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PartnerId { get; set; }
    
    public string FileReference { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    
    public string StorageKey { get; set; } = string.Empty; // S3/MinIO reference
    public string ContentHashSha256 { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    
    public string Status { get; set; } = "Received"; // Received, Validated, Processing, Processed, Rejected, Quarantined, Failed
    
    public string? ErrorCode { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationPartner Partner { get; set; } = null!;
}

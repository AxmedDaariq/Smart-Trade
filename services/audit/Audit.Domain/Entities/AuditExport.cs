namespace Audit.Domain.Entities;

public class AuditExport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ExportReference { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    
    public string Purpose { get; set; } = string.Empty;
    public string? FilterJson { get; set; } // JSONB
    
    public string Format { get; set; } = "CSV"; // CSV, JSON, PDF
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, Expired
    
    public long? RecordCount { get; set; }
    public string? StorageReference { get; set; }
    public string? ContentHash { get; set; }
    
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}

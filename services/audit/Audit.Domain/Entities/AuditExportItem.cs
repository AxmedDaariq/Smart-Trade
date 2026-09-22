namespace Audit.Domain.Entities;

public class AuditExportItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid ExportId { get; set; }
    public Guid AuditEventId { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditExport Export { get; set; } = null!;
}

namespace Audit.Domain.Entities;

public class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    
    public Guid? MessageId { get; set; }
    public string RequestHash { get; set; } = string.Empty;
    
    public int? ResponseStatus { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
}

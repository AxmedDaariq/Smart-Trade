namespace Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    
    public string TokenHash { get; set; } = string.Empty;
    public Guid TokenFamilyId { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    
    public string? RevocationReason { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    
    public string CreatedFromIp { get; set; } = string.Empty;
    public string CreatedFromUserAgent { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Active"; // Active, Used, Revoked, Expired, ReuseDetected
    
    public Session Session { get; set; } = null!;
}

namespace Identity.Domain.Entities;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    
    public string SessionReference { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevocationReason { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Expired, Revoked
    public string CorrelationId { get; set; } = string.Empty;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

namespace Identity.Domain.Entities;

public class PasswordResetToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Used, Expired, Revoked
    
    public string RequestedFromIp { get; set; } = string.Empty;
    public string RequestedFromUserAgent { get; set; } = string.Empty;
}

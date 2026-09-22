namespace Identity.Domain.Entities;

public class EmailVerificationToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Used, Expired, Revoked
}

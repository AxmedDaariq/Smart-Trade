namespace Identity.Domain.Entities;

public class UserSecurityState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public int FailedLoginCount { get; set; }
    public DateTime? LastFailedLoginAtUtc { get; set; }
    public DateTime? LastSuccessfulLoginAtUtc { get; set; }
    public DateTime? LockedUntilUtc { get; set; }
    
    public bool PasswordCompromised { get; set; }
    public bool RequirePasswordChange { get; set; }
    public bool RequireMfaEnrollment { get; set; }
    
    public string SecurityRiskLevel { get; set; } = "Normal"; // Normal, Elevated, High, Critical
    
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public User User { get; set; } = null!;
}

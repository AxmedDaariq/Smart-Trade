namespace Identity.Domain.Entities;

public class MfaConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    
    public string Method { get; set; } = "TOTP";
    public string SecretEncrypted { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    
    public DateTime? VerifiedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAtUtc { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public ICollection<MfaRecoveryCode> RecoveryCodes { get; set; } = new List<MfaRecoveryCode>();
}

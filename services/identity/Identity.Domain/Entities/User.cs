namespace Identity.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    
    public string? PhoneNumber { get; set; }
    public bool PhoneVerified { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordAlgorithm { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Active, Locked, Suspended, Disabled, Archived
    
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? LastFailedLoginAtUtc { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockedUntilUtc { get; set; }
    public DateTime? PasswordChangedAtUtc { get; set; }
    
    public bool MfaEnabled { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

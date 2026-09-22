namespace Identity.Domain.Entities;

public class MfaRecoveryCode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MfaConfigurationId { get; set; }
    
    public string CodeHash { get; set; } = string.Empty;
    
    public DateTime? UsedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Active"; // Active, Used
    
    public MfaConfiguration MfaConfiguration { get; set; } = null!;
}

namespace Ectn.Domain.Entities;

public class EctnVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string EctnNumber { get; set; } = string.Empty;
    public string VerificationReference { get; set; } = string.Empty;
    
    public DateTime VerifiedAtUtc { get; set; } = DateTime.UtcNow;
    public string VerificationResult { get; set; } = string.Empty; // Valid, Invalid, Expired, Cancelled, NotFound, Superseded
    
    public string Source { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public Guid? UserId { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

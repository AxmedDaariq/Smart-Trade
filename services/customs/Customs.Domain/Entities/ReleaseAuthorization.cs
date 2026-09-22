namespace Customs.Domain.Entities;

public class ReleaseAuthorization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    public Guid ClearanceId { get; set; }
    
    public string AuthorizationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Authorized, Expired, Revoked, Used
    
    public Guid? AuthorizedBy { get; set; }
    public DateTime? AuthorizedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    
    public string ConditionsJson { get; set; } = "{}"; // JSONB
    
    public Guid? RevokedBy { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevocationReason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    public CustomsClearance Clearance { get; set; } = null!;
}

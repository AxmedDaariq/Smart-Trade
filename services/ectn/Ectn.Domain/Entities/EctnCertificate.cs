namespace Ectn.Domain.Entities;

public class EctnCertificate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid EctnApplicationId { get; set; }
    
    public string EctnNumber { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    
    public DateTime IssuedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Cancelled, Expired, Superseded
    public int CertificateVersion { get; set; } = 1;
    
    public Guid? QrCodeId { get; set; }
    public string DocumentReference { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

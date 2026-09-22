namespace Ectn.Domain.Entities;

public class EctnQrCode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnCertificateId { get; set; }
    
    public int QrVersion { get; set; } = 1;
    public string PayloadReference { get; set; } = string.Empty;
    public string PayloadHash { get; set; } = string.Empty;
    
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Revoked
    
    public EctnCertificate EctnCertificate { get; set; } = null!;
}

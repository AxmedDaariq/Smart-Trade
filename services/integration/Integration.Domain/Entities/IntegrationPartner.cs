namespace Integration.Domain.Entities;

public class IntegrationPartner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string PartnerReference { get; set; } = string.Empty;
    public string PartnerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string PartnerType { get; set; } = string.Empty; // Government, Bank, MobileMoney, PaymentProvider, ECTNProvider, CustomsSystem, GPSProvider, AISProvider, ANPRProvider, PortAuthority, BorderAuthority, TradePartner, LogisticsCompany, TMA, InternalService, Other
    
    public string? Description { get; set; }
    public string? CountryCode { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Active, Suspended, Inactive, Archived
    public string TrustLevel { get; set; } = "Standard"; // Low, Standard, Trusted, HighlyTrusted
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<IntegrationSystem> Systems { get; set; } = new List<IntegrationSystem>();
}

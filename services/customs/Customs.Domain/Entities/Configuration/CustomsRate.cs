namespace Customs.Domain.Entities.Configuration;

public class CustomsRate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid ChargeTypeId { get; set; }
    
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RateType { get; set; } = string.Empty;
    
    public Guid? CurrentVersionId { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Active, Suspended, Retired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<CustomsRateVersion> Versions { get; set; } = new List<CustomsRateVersion>();
    public CustomsChargeType ChargeType { get; set; } = null!;
}

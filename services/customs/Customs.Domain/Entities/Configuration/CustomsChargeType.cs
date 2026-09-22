namespace Customs.Domain.Entities.Configuration;

public class CustomsChargeType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty; // CustomsDuty, CustomsLevy, Excise, ImportCharge, OtherCustomsCharge
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

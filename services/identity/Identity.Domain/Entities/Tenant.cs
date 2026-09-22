namespace Identity.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TenantType { get; set; } = string.Empty; // e.g., Government, LogisticsCompany
    public string Status { get; set; } = "Pending";
    public string CountryCode { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

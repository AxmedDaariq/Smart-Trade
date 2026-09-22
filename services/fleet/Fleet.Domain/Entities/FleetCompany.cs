namespace Fleet.Domain.Entities;

public class FleetCompany
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid OrganizationId { get; set; } // Link to Identity Organization
    
    public string FleetCode { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradingName { get; set; } = string.Empty;
    
    public string RegistrationNumber { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Active, Suspended, Inactive, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}

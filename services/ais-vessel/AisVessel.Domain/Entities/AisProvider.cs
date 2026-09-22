namespace AisVessel.Domain.Entities;

public class AisProvider
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ProviderReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string ProviderType { get; set; } = string.Empty; // Commercial, Government, PortAuthority, Internal, Other
    
    public string EndpointReference { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    
    public string ConfigurationVersion { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

namespace Integration.Domain.Entities;

public class IntegrationExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string InternalSystem { get; set; } = string.Empty; // e.g. Payment, Ectn, Vehicle
    public string InternalResourceType { get; set; } = string.Empty;
    public Guid InternalResourceId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty; // e.g. BankTransactionReference, GPSProviderVehicleId
    public string ExternalResourceType { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    
    public string? ExternalStatus { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

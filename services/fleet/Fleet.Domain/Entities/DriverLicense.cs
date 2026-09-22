namespace Fleet.Domain.Entities;

public class DriverLicense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DriverId { get; set; }
    
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty; // HeavyVehicle, Truck, Trailer, Commercial, DangerousGoods, Other
    
    public string IssuingCountryCode { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    
    public DateTime IssueDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Valid, Expired, Suspended, Revoked
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Driver Driver { get; set; } = null!;
}

namespace Fleet.Domain.Entities;

public class VehicleRegistration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VehicleId { get; set; }
    
    public string RegistrationNumber { get; set; } = string.Empty;
    public string RegistrationCountryCode { get; set; } = string.Empty;
    public string RegistrationAuthority { get; set; } = string.Empty;
    
    public DateTime IssueDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Valid, Expired, Suspended, Cancelled
    
    public string DocumentReference { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vehicle Vehicle { get; set; } = null!;
}

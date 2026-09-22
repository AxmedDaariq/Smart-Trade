namespace Fleet.Domain.Entities;

public class DriverCompliance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DriverId { get; set; }
    
    public string ComplianceType { get; set; } = string.Empty; // Medical, License, Professional, CrossBorder, Safety, Training, Other
    public string ReferenceNumber { get; set; } = string.Empty;
    
    public DateTime IssueDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Valid, Expiring, Expired, Suspended, Rejected
    public string Authority { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Driver Driver { get; set; } = null!;
}

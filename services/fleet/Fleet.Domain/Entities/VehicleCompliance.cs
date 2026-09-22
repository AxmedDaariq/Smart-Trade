namespace Fleet.Domain.Entities;

public class VehicleCompliance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Might be assigned to a vehicle or a trailer
    public Guid? VehicleId { get; set; }
    public Guid? TrailerId { get; set; }
    
    public string ComplianceType { get; set; } = string.Empty; // Roadworthiness, Insurance, Inspection, Registration, Permit, Emissions, Safety, Other
    public string ComplianceNumber { get; set; } = string.Empty;
    
    public DateTime IssueDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Valid, Expiring, Expired, Suspended, Rejected
    public string Authority { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vehicle? Vehicle { get; set; }
    public Trailer? Trailer { get; set; }
}

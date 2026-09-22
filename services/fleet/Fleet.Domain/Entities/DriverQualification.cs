namespace Fleet.Domain.Entities;

public class DriverQualification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DriverId { get; set; }
    
    public string QualificationType { get; set; } = string.Empty; // ProfessionalDriver, DangerousGoods, Hazmat, CrossBorder, RefrigeratedCargo, ContainerHandling, Other
    public string QualificationNumber { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    
    public DateTime IssueDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Valid, Expired, Suspended, Revoked
    
    public string DocumentReference { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Driver Driver { get; set; } = null!;
}

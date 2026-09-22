namespace Fleet.Domain.Entities;

public class DriverVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DriverId { get; set; }
    
    public string VerificationType { get; set; } = string.Empty; // Identity, License, Employment, Background, CrossBorder, Professional, Other
    public string VerificationReference { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected, Expired
    
    public Guid? VerifiedBy { get; set; }
    public DateTime? VerifiedAtUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
}

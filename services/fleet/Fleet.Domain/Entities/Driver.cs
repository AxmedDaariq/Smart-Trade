namespace Fleet.Domain.Entities;

public class Driver
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    public Guid? UserId { get; set; }
    
    public string DriverReference { get; set; } = string.Empty;
    public string? EmployeeNumber { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Active, Suspended, Inactive, Blocked, Archived
    public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<DriverLicense> Licenses { get; set; } = new List<DriverLicense>();
    public ICollection<DriverQualification> Qualifications { get; set; } = new List<DriverQualification>();
    public ICollection<DriverCompliance> Compliances { get; set; } = new List<DriverCompliance>();
    public ICollection<DriverVerification> Verifications { get; set; } = new List<DriverVerification>();
    public ICollection<DriverDocument> Documents { get; set; } = new List<DriverDocument>();
    public ICollection<DriverCapability> Capabilities { get; set; } = new List<DriverCapability>();
    public ICollection<DriverAssignment> Assignments { get; set; } = new List<DriverAssignment>();
}

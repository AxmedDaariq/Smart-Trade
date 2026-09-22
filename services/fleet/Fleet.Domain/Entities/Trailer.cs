namespace Fleet.Domain.Entities;

public class Trailer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    
    public string TrailerReference { get; set; } = string.Empty;
    public string TrailerType { get; set; } = string.Empty; // ContainerChassis, Flatbed, Tanker, Refrigerated, Curtainsider, Lowbed, Other
    
    public string LicensePlate { get; set; } = string.Empty;
    public string NormalizedLicensePlate { get; set; } = string.Empty;
    
    public string VIN { get; set; } = string.Empty;
    public string ChassisNumber { get; set; } = string.Empty;
    
    public string Model { get; set; } = string.Empty;
    public int ModelYear { get; set; }
    
    public decimal PayloadCapacity { get; set; }
    public decimal GrossVehicleWeight { get; set; }
    public decimal TareWeight { get; set; }
    public string WeightUnit { get; set; } = "kg";
    
    public string Status { get; set; } = "Draft"; // Draft, PendingVerification, Active, Inactive, Suspended, UnderMaintenance, Held, Retired
    public string OwnershipType { get; set; } = string.Empty;
    public string CountryOfRegistration { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<VehicleDocument> Documents { get; set; } = new List<VehicleDocument>();
    public ICollection<VehicleCompliance> Compliances { get; set; } = new List<VehicleCompliance>();
    public ICollection<VehicleCapability> Capabilities { get; set; } = new List<VehicleCapability>();
    public ICollection<TrailerAssignment> Assignments { get; set; } = new List<TrailerAssignment>();
}

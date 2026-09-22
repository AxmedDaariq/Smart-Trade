namespace Fleet.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty; // Truck, TractorUnit, RigidTruck, Pickup, Van, Other
    public string VehicleCategory { get; set; } = string.Empty; // Heavy, Medium, Light, SpecialPurpose, Other
    
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int ModelYear { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    
    public string VIN { get; set; } = string.Empty;
    public string ChassisNumber { get; set; } = string.Empty;
    public string EngineNumber { get; set; } = string.Empty;
    
    public string LicensePlate { get; set; } = string.Empty;
    public string NormalizedLicensePlate { get; set; } = string.Empty;
    
    public string Color { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty; // Diesel, Petrol, Electric, Hybrid, Other
    public string OwnershipType { get; set; } = string.Empty; // Owned, Leased, Rented, Contracted, Other
    
    public string Status { get; set; } = "Draft"; // Draft, PendingVerification, Active, Inactive, Suspended, UnderMaintenance, Held, Retired
    public string CountryOfRegistration { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Truck? Truck { get; set; }
    public ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();
    public ICollection<VehicleCompliance> Compliances { get; set; } = new List<VehicleCompliance>();
    public ICollection<VehicleInspection> Inspections { get; set; } = new List<VehicleInspection>();
    public ICollection<VehicleDocument> Documents { get; set; } = new List<VehicleDocument>();
    public ICollection<VehicleCapability> Capabilities { get; set; } = new List<VehicleCapability>();
    public ICollection<VehicleAssignment> Assignments { get; set; } = new List<VehicleAssignment>();
}

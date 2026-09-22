namespace Fleet.Domain.Entities;

public class DispatchRequirement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DispatchOrderId { get; set; }
    
    public string RequirementType { get; set; } = string.Empty; // CustomsRelease, Declaration, ECTN, VehicleCompliance, DriverCompliance, Insurance, Permit, Inspection, Other
    public string RequirementReference { get; set; } = string.Empty;
    
    public bool IsRequired { get; set; } = true;
    public string Status { get; set; } = "Pending"; // Pending, Satisfied, Failed, Waived
    
    public DateTime? ValidatedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DispatchOrder DispatchOrder { get; set; } = null!;
}

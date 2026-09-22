namespace Ectn.Domain.Entities;

public class EctnValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public string ValidationType { get; set; } = string.Empty; // Schema, BusinessRule, Document, Party, Cargo, Vessel, Duplicate, Reference, External, Other
    public string Status { get; set; } = "Pending"; // Pending, Passed, Failed, Warning
    
    public string ValidatedBy { get; set; } = string.Empty;
    public DateTime ValidatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string FailureReason { get; set; } = string.Empty;
    
    public string ValidationDetailsJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

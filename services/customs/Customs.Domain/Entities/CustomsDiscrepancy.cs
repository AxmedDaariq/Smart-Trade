namespace Customs.Domain.Entities;

public class CustomsDiscrepancy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    
    public Guid? InspectionOrderId { get; set; }
    
    public string DiscrepancyType { get; set; } = string.Empty; // Weight, Quantity, Seal, HSClassification, Document, Value, Container, Cargo, Other
    public string Description { get; set; } = string.Empty;
    
    public string DeclaredValue { get; set; } = string.Empty;
    public string ObservedValue { get; set; } = string.Empty;
    
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Status { get; set; } = "Open"; // Open, UnderReview, Resolved, Accepted, Escalated
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? ResolutionReason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
}

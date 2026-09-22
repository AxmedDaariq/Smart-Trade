namespace AnprGate.Domain.Entities;

public class AnprManualOverride
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Authorization, Gate, Passage, Border, Recognition, Other
    public Guid ResourceId { get; set; }
    
    public string OverrideType { get; set; } = string.Empty;
    
    public string? PreviousDecision { get; set; }
    public string NewDecision { get; set; } = string.Empty;
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public Guid PerformedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    
    public DateTime PerformedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string? EvidenceReference { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

namespace AnprGate.Domain.Entities;

public class GateAuthorizationDecision
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid AuthorizationRequestId { get; set; }
    
    public string Decision { get; set; } = string.Empty; // Approved, Denied, ManualReview
    public string DecisionCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    
    public string PolicyVersion { get; set; } = string.Empty;
    
    public DateTime DecisionAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? DecidedBy { get; set; }
    
    public string Source { get; set; } = string.Empty; // SystemPolicy, CustomsReference, ManualOfficer, Integration
    
    public DateTime? ExpiresAtUtc { get; set; }
    public string? ConditionsJson { get; set; } // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GateAuthorizationRequest AuthorizationRequest { get; set; } = null!;
}

namespace AnprGate.Domain.Entities;

public class GateAuthorizationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string AuthorizationReference { get; set; } = string.Empty;
    
    public Guid? RecognitionEventId { get; set; }
    
    public string? VehicleReference { get; set; } // External to Fleet
    public string? DriverReference { get; set; } // External to Fleet
    public string? TripReference { get; set; } // External to Corridor
    public string? DispatchReference { get; set; }
    
    public Guid GateId { get; set; }
    public Guid? LaneId { get; set; }
    public string Direction { get; set; } = string.Empty;
    
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? RequestedBy { get; set; }
    
    public string Status { get; set; } = "Requested"; // Requested, Evaluating, Approved, Denied, Expired, Cancelled, ManualReview
    
    public DateTime? ExpiresAtUtc { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Gate Gate { get; set; } = null!;
    public GateLane? Lane { get; set; }
    public AnprRecognitionEvent? RecognitionEvent { get; set; }
    
    public ICollection<GateAuthorizationDecision> Decisions { get; set; } = new List<GateAuthorizationDecision>();
}

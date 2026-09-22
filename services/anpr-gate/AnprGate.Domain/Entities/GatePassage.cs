namespace AnprGate.Domain.Entities;

public class GatePassage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string PassageReference { get; set; } = string.Empty;
    
    public Guid GateId { get; set; }
    public Guid? LaneId { get; set; }
    
    public Guid? RecognitionEventId { get; set; }
    public Guid? AuthorizationRequestId { get; set; }
    
    public string? VehicleReference { get; set; } // External to Fleet
    public string? DriverReference { get; set; } // External to Fleet
    public string? TripReference { get; set; } // External to Corridor
    
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    
    public DateTime? EntryAtUtc { get; set; }
    public DateTime? ExitAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Authorized, Denied, Entered, Exited, Completed, Cancelled, ManualReview
    
    public string? EvidenceReference { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Gate Gate { get; set; } = null!;
    public GateLane? Lane { get; set; }
    public AnprRecognitionEvent? RecognitionEvent { get; set; }
    public GateAuthorizationRequest? AuthorizationRequest { get; set; }
}

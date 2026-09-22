namespace AnprGate.Domain.Entities;

public class GateEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GateId { get; set; }
    public Guid? LaneId { get; set; }
    
    public string EventType { get; set; } = string.Empty; // GateOpened, GateClosed, LaneOpened, LaneClosed, PassageStarted, PassageCompleted, EmergencyMode, ManualMode, SystemFault, Recovery
    
    public DateTime OccurredAtUtc { get; set; }
    
    public string Source { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public string? Description { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Gate Gate { get; set; } = null!;
    public GateLane? Lane { get; set; }
}

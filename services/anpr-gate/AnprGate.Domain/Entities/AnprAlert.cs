using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class AnprAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string AlertReference { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty; 
    // UnknownPlate, LowConfidence, DuplicateRecognition, ReplayDetected, CameraOffline, EvidenceTampering, UnauthorizedPassage, AuthorizationDenied, GateFailure, BorderMismatch, DirectionMismatch, ExpiredAuthorization, Other
    
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    
    public Guid? RecognitionEventId { get; set; }
    public Guid? GatePassageId { get; set; }
    public Guid? BorderCrossingId { get; set; }
    public Guid? CameraId { get; set; }
    public Guid? GateId { get; set; }
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

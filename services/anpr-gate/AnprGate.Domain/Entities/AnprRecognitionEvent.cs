namespace AnprGate.Domain.Entities;

public class AnprRecognitionEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CameraId { get; set; }
    public Guid? GateId { get; set; }
    public Guid? LaneId { get; set; }
    
    public string RecognitionReference { get; set; } = string.Empty;
    public string? ProviderEventId { get; set; }
    
    public string? OriginalPlateText { get; set; }
    public string NormalizedPlateText { get; set; } = string.Empty;
    
    public string? CountryCode { get; set; }
    public double? Confidence { get; set; }
    
    public DateTime RecognizedAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public int? SequenceNumber { get; set; }
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound, Unknown
    
    public string VehicleMatchStatus { get; set; } = "Unmatched"; // Matched, Unmatched, ManualReview, Rejected
    public string TripMatchStatus { get; set; } = "Unmatched"; // Matched, Unmatched, ManualReview, Rejected
    
    public string? EvidenceReference { get; set; }
    public string? EvidenceHash { get; set; }
    
    public string Source { get; set; } = string.Empty; // Camera, Provider, Mobile, Manual, Integration
    public string CorrelationId { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Received"; // Received, Validated, Rejected, Matched, Unmatched, ManualReview, Processed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprCamera Camera { get; set; } = null!;
    public Gate? Gate { get; set; }
    public GateLane? Lane { get; set; }
    
    public ICollection<PlateObservation> PlateObservations { get; set; } = new List<PlateObservation>();
    public ICollection<AnprVehicleMatch> VehicleMatches { get; set; } = new List<AnprVehicleMatch>();
    public ICollection<AnprTripMatch> TripMatches { get; set; } = new List<AnprTripMatch>();
    public ICollection<RecognitionEvidence> Evidences { get; set; } = new List<RecognitionEvidence>();
}

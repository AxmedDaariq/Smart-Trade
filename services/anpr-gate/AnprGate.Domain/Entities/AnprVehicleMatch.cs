namespace AnprGate.Domain.Entities;

public class AnprVehicleMatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RecognitionEventId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty; // External to Fleet
    public string? FleetCompanyReference { get; set; }
    
    public DateTime MatchedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string MatchMethod { get; set; } = string.Empty;
    public double? MatchConfidence { get; set; }
    
    public string Status { get; set; } = "Matched"; // Matched, Unmatched, ManualReview, Rejected
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprRecognitionEvent RecognitionEvent { get; set; } = null!;
}

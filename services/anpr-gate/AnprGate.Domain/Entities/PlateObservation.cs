using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class PlateObservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RecognitionEventId { get; set; }
    
    public string NormalizedPlateText { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public double? Confidence { get; set; }
    
    public DateTime ObservedAtUtc { get; set; }
    
    public Point? Position { get; set; } // geography(Point,4326)
    public string Direction { get; set; } = string.Empty;
    
    public string? MatchMethod { get; set; } // Exact, NormalizedExact, ProviderMatch, Manual, FuzzyCandidate
    public double? MatchConfidence { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprRecognitionEvent RecognitionEvent { get; set; } = null!;
}

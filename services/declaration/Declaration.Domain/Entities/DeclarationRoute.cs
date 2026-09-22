namespace Declaration.Domain.Entities;

public class DeclarationRoute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    
    public int SequenceNumber { get; set; }
    
    public string LocationType { get; set; } = string.Empty; // Origin, Port, Border, TransitPoint, DryPort, Destination, Other
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    
    public DateTime? PlannedArrivalUtc { get; set; }
    public DateTime? PlannedDepartureUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
}

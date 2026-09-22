namespace Declaration.Domain.Entities;

public class DeclarationTransport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    
    public string TransportMode { get; set; } = string.Empty; // Sea, Road, Rail, Air, Multimodal
    
    public string CarrierReference { get; set; } = string.Empty;
    public string CarrierName { get; set; } = string.Empty;
    
    public string? VehicleReference { get; set; }
    public string? TrailerReference { get; set; }
    public string? VesselReference { get; set; }
    public string? VoyageReference { get; set; }
    
    public string PortOfLoading { get; set; } = string.Empty;
    public string PortOfDischarge { get; set; } = string.Empty;
    
    public string OriginCountryCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    
    public string BorderPoint { get; set; } = string.Empty;
    public string FinalDestination { get; set; } = string.Empty;
    
    public DateTime? EstimatedDepartureUtc { get; set; }
    public DateTime? EstimatedArrivalUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
}

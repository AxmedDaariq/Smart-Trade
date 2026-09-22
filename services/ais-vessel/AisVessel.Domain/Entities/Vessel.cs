namespace AisVessel.Domain.Entities;

public class Vessel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string VesselReference { get; set; } = string.Empty;
    public string? ImoNumber { get; set; }
    public string? MmsiNumber { get; set; }
    public string? CallSign { get; set; }
    
    public string VesselName { get; set; } = string.Empty;
    public string VesselType { get; set; } = string.Empty;
    public string? FlagCountry { get; set; }
    
    public int? BuildYear { get; set; }
    public double? GrossTonnage { get; set; }
    public double? NetTonnage { get; set; }
    public double? DeadweightTonnage { get; set; }
    public double? LengthMeters { get; set; }
    public double? BeamMeters { get; set; }
    public double? DraftMeters { get; set; }
    
    public string Status { get; set; } = "Unknown"; // Unknown, Expected, Arriving, InPort, Anchored, Departing, AtSea, Delayed, Cancelled, Inactive
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<VesselIdentifier> Identifiers { get; set; } = new List<VesselIdentifier>();
    public ICollection<VesselCharacteristic> Characteristics { get; set; } = new List<VesselCharacteristic>();
}

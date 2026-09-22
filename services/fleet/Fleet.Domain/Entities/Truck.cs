namespace Fleet.Domain.Entities;

public class Truck
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VehicleId { get; set; }
    
    public string TruckType { get; set; } = string.Empty; // Tractor, Rigid, Tanker, Flatbed, ContainerCarrier, Refrigerated, Other
    
    public int AxleCount { get; set; }
    
    public decimal GrossVehicleWeight { get; set; }
    public decimal TareWeight { get; set; }
    public decimal PayloadCapacity { get; set; }
    public string WeightUnit { get; set; } = "kg";
    
    public string CabType { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vehicle Vehicle { get; set; } = null!;
}

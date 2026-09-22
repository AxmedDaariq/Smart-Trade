namespace Ectn.Domain.Entities;

public class EctnReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid EctnApplicationId { get; set; }
    
    public string ReferenceType { get; set; } = string.Empty; // ECTN, Application, Certificate, ExternalECTN, ShipmentReference, BookingReference
    public string ReferenceValue { get; set; } = string.Empty;
    public string NormalizedValue { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}

namespace Customs.Domain.Entities;

public class CustomsReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CustomsCaseId { get; set; }
    
    public string ReferenceType { get; set; } = string.Empty; // Declaration, ECTN, Shipment, Container, BillOfLading, ExternalCase, Payment, Release, Other
    public string ReferenceValue { get; set; } = string.Empty;
    public string NormalizedValue { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsCase CustomsCase { get; set; } = null!;
}

namespace Declaration.Domain.Entities;

public class DeclarationReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    public Guid TenantId { get; set; }
    
    public string ReferenceType { get; set; } = string.Empty; // Declaration, ECTN, Shipment, Invoice, BillOfLading, Booking, Container, ExternalDeclaration, Transit, Other
    public string ReferenceValue { get; set; } = string.Empty;
    public string NormalizedValue { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}

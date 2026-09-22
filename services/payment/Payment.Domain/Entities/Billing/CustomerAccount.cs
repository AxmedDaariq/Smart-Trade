namespace Payment.Domain.Entities.Billing;

public class CustomerAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid OrganizationId { get; set; } // Link to Identity Service
    
    public string AccountNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxIdentifier { get; set; } = string.Empty; // e.g. VAT or TIN
    
    public decimal CreditLimit { get; set; }
    public decimal AvailableCredit { get; set; }
    public decimal CurrentBalance { get; set; }
    
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Active"; // Active, Suspended, Closed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

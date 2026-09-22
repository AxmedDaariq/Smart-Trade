namespace Payment.Domain.Entities.Billing;

public class InvoiceLineItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    public string CurrencyCode { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Invoice Invoice { get; set; } = null!;
}

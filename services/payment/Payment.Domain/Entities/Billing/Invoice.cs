namespace Payment.Domain.Entities.Billing;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CustomerAccountId { get; set; }
    
    public string InvoiceNumber { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty; // Customs, ECTN
    public string SourceReferenceId { get; set; } = string.Empty; // e.g., CustomsAssessmentId
    
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Draft"; // Draft, Issued, PartiallyPaid, Paid, Cancelled, Overdue, Refunded
    
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueDateUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public CustomerAccount CustomerAccount { get; set; } = null!;
}

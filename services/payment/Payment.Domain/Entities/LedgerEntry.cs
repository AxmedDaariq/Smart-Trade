namespace Payment.Domain.Entities;

public class LedgerEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid CustomerAccountId { get; set; }
    
    public Guid? PaymentTransactionId { get; set; }
    public Guid? InvoiceId { get; set; }
    
    public string EntryType { get; set; } = string.Empty; // Debit, Credit
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime PostedAtUtc { get; set; } = DateTime.UtcNow;
    
    // Immutability hash for ledger integrity
    public string Hash { get; set; } = string.Empty; 
    public string PreviousHash { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

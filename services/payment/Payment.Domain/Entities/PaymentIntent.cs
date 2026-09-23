using System;
using System.Collections.Generic;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Entities;

public class PaymentIntent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? CustomerAccountId { get; set; }
    
    public string IntentReference { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty; // Declaration, Customs, ECTN
    public string SourceReferenceId { get; set; } = string.Empty; // e.g. CustomsAssessmentId
    public Guid? CustomsCaseId { get; set; }
    public string DeclarationNumber { get; set; } = string.Empty;
    public string ECTNNumber { get; set; } = string.Empty;
    
    public decimal AmountRequested { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Created"; // Created, RequiresPaymentMethod, Processing, Succeeded, Canceled, Failed
    
    public Guid? GatewayConfigurationId { get; set; }
    public string GatewayIntentId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? DueDate { get; set; }
    
    public string MetadataJson { get; set; } = "{}"; // JSONB
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();

    public PaymentIntent() { }

    public PaymentIntent(
        Guid tenantId,
        string intentReference,
        string sourceSystem,
        string sourceReferenceId,
        Guid customsCaseId,
        string declarationNumber,
        string ectnNumber,
        Money money,
        DateTime? dueDate = null)
    {
        TenantId = tenantId;
        IntentReference = intentReference;
        SourceSystem = sourceSystem;
        SourceReferenceId = sourceReferenceId;
        CustomsCaseId = customsCaseId;
        DeclarationNumber = declarationNumber ?? string.Empty;
        ECTNNumber = ectnNumber ?? string.Empty;
        AmountRequested = money.Amount;
        CurrencyCode = money.Currency;
        Status = "Created";
        DueDate = dueDate;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Process()
    {
        if (Status != "Created" && Status != "RequiresPaymentMethod")
            throw new InvalidOperationException($"Cannot process payment intent in state '{Status}'.");
        Status = "Processing";
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkSucceeded(PaymentTransaction transaction)
    {
        if (Status == "Succeeded") return;
        if (Status == "Canceled") throw new InvalidOperationException("Cannot succeed a canceled payment intent.");

        Status = "Succeeded";
        UpdatedAtUtc = DateTime.UtcNow;
        if (transaction != null && !Transactions.Contains(transaction))
        {
            Transactions.Add(transaction);
        }
    }

    public void MarkFailed(string reason)
    {
        Status = "Failed";
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == "Succeeded") throw new InvalidOperationException("Cannot cancel an already succeeded payment.");
        Status = "Canceled";
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

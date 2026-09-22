namespace SmartTrade.Analytics.Domain.Entities;

public class FactTrip
{
    public Guid TripKey { get; set; }
    public Guid TripId { get; set; }
    public Guid TenantKey { get; set; }
    public Guid VehicleKey { get; set; }
    public Guid CorridorKey { get; set; }
    public int DateKey { get; set; }
    
    public decimal PlannedDistance { get; set; }
    public decimal ActualDistance { get; set; }
    public decimal PlannedDuration { get; set; }
    public decimal ActualDuration { get; set; }
    public decimal DelayMinutes { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public bool Cancelled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class FactPaymentOperational
{
    public Guid PaymentKey { get; set; }
    public Guid TenantKey { get; set; }
    public int DateKey { get; set; }
    
    public int TransactionCount { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public int RefundCount { get; set; }
    
    public decimal TotalAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal SettlementAmount { get; set; }
    public string Currency { get; set; } = "USD";
}

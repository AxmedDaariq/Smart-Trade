namespace Corridor.Domain.Entities;

public class TripHold
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string HoldType { get; set; } = string.Empty; // Operational, Security, Documentation, CustomsReference, PaymentReference, Vehicle, Driver, Border, Other
    public string Reason { get; set; } = string.Empty;
    
    public Guid PlacedBy { get; set; }
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ReleasedBy { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Released, Cancelled, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}

namespace Fleet.Domain.Entities;

public class FleetHold
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty; // Vehicle, Driver, Trailer, Dispatch
    public Guid ResourceId { get; set; }
    
    public string HoldType { get; set; } = string.Empty; // Compliance, Safety, Legal, Operational, Documentation, Maintenance, Other
    public string Reason { get; set; } = string.Empty;
    
    public Guid PlacedBy { get; set; }
    public DateTime PlacedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ReleasedBy { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Released, Cancelled, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

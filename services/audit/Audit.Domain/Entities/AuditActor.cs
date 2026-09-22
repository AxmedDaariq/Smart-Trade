namespace Audit.Domain.Entities;

public class AuditActor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ActorType { get; set; } = string.Empty;
    public string ActorReference { get; set; } = string.Empty;
    
    public string? DisplayReference { get; set; }
    public string? OrganizationReference { get; set; }
    public string? ServiceName { get; set; }
    public string? DeviceReference { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

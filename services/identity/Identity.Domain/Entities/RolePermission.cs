namespace Identity.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid AssignedBy { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Revoked
    
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

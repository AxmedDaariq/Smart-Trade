namespace Identity.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid AssignedBy { get; set; }
    
    public DateTime? ExpiresAtUtc { get; set; }
    public string Status { get; set; } = "Active"; // Active, Suspended, Expired, Revoked
    
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

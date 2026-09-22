namespace Identity.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

namespace Identity.Domain.Entities;

public class PasswordHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordAlgorithm { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

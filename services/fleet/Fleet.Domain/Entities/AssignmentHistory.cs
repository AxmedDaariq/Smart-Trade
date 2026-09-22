namespace Fleet.Domain.Entities;

public class AssignmentHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string AssignmentType { get; set; } = string.Empty; // Vehicle, Driver, Trailer
    public Guid AssignmentId { get; set; }
    
    public string Action { get; set; } = string.Empty;
    
    public string PreviousValueJson { get; set; } = "{}"; // JSONB
    public string NewValueJson { get; set; } = "{}"; // JSONB
    
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Reason { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
}

namespace Corridor.Domain.Entities;

public class CorridorVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CorridorId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Published, Superseded, Archived
    public string? Description { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public Guid? PublishedBy { get; set; }

    public Corridor Corridor { get; set; } = null!;
    public ICollection<CorridorSegment> Segments { get; set; } = new List<CorridorSegment>();
}

namespace Customs.Domain.Entities.Configuration;

public class TariffSchedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ScheduleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string ProcedureScope { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Published, Superseded, Archived
    
    public int CurrentVersionNumber { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<TariffScheduleVersion> Versions { get; set; } = new List<TariffScheduleVersion>();
}

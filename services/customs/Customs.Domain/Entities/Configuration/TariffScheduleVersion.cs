namespace Customs.Domain.Entities.Configuration;

public class TariffScheduleVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TariffScheduleId { get; set; }
    
    public int VersionNumber { get; set; }
    public string Status { get; set; } = "Draft"; 
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string? LegalBasis { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
    
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public Guid? PublishedBy { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public TariffSchedule TariffSchedule { get; set; } = null!;
    public ICollection<TariffItem> TariffItems { get; set; } = new List<TariffItem>();
}

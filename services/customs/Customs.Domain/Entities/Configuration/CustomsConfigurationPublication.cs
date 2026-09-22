namespace Customs.Domain.Entities.Configuration;

public class CustomsConfigurationPublication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string ConfigurationType { get; set; } = string.Empty; 
    public Guid ConfigurationId { get; set; }
    public int ConfigurationVersion { get; set; }
    
    public Guid PublishedBy { get; set; }
    public DateTime PublishedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, Published, Superseded, Cancelled
    public string PublicationReference { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

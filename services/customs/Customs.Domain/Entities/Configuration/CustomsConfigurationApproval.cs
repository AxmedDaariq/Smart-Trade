namespace Customs.Domain.Entities.Configuration;

public class CustomsConfigurationApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string ConfigurationType { get; set; } = string.Empty;
    public Guid ConfigurationId { get; set; }
    public int VersionNumber { get; set; }
    
    public Guid SubmittedBy { get; set; }
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    
    public string Decision { get; set; } = "Submitted"; // Submitted, Approved, Rejected, Returned
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

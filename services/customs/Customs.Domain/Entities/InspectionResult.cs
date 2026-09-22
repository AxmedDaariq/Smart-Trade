namespace Customs.Domain.Entities;

public class InspectionResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InspectionOrderId { get; set; }
    
    public string Result { get; set; } = string.Empty; // Pass, PassWithDiscrepancy, Fail, UnableToComplete
    public string Findings { get; set; } = string.Empty;
    
    public int? VerifiedPackageCount { get; set; }
    public decimal? VerifiedGrossWeight { get; set; }
    public decimal? VerifiedNetWeight { get; set; }
    
    public bool SealVerified { get; set; }
    public bool DocumentVerified { get; set; }
    public bool DiscrepancyFound { get; set; }
    
    public string Action { get; set; } = string.Empty;
    
    public Guid OfficerId { get; set; }
    
    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public InspectionOrder InspectionOrder { get; set; } = null!;
}

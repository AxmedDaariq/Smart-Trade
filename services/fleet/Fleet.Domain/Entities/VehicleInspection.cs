namespace Fleet.Domain.Entities;

public class VehicleInspection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VehicleId { get; set; }
    
    public string InspectionReference { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty; // PreTrip, Periodic, Safety, Roadworthiness, PostIncident, Maintenance, Other
    
    public DateTime InspectionDateUtc { get; set; }
    public DateTime ExpiryDateUtc { get; set; }
    
    public string InspectorReference { get; set; } = string.Empty;
    
    public decimal OdometerReading { get; set; }
    public string OdometerUnit { get; set; } = "km";
    
    public string Result { get; set; } = "Pass"; // Pass, PassWithMinorIssues, Fail
    public string Findings { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vehicle Vehicle { get; set; } = null!;
}

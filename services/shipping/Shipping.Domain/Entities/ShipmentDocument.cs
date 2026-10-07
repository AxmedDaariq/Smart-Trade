namespace Shipping.Domain.Entities;

/// <summary>
/// A shipping document attached to a shipment (Bill of Lading, Packing List, Certificate of Origin, etc.).
/// </summary>
public class ShipmentDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    /// <summary>Document type code (e.g. BL, AWB, PL, COO, CI, MSDS).</summary>
    public string DocumentType { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;

    /// <summary>Storage URI (blob storage / object storage reference).</summary>
    public string StorageUri { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/pdf";
    public long FileSizeBytes { get; set; }

    public bool IsOriginal { get; set; }
    public DateTime? IssuedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public string? IssuedBy { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid UploadedBy { get; set; }
}

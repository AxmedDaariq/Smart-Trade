using System;
using AnprGate.Domain.Enums;

namespace AnprGate.Domain.Entities;

public class RecognitionEvent
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CameraId { get; private set; }
    public Guid? GateId { get; private set; }
    public Guid? LaneId { get; private set; }
    
    public DateTime ObservedAt { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    
    public string PlateText { get; private set; }
    public string NormalizedPlateText { get; private set; }
    public string CountryCode { get; private set; }
    
    public double Confidence { get; private set; }
    public Direction Direction { get; private set; }
    
    public RecognitionStatus Status { get; private set; }
    public string ProviderEventId { get; private set; }
    public Guid? ImageEvidenceReferenceId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    protected RecognitionEvent() { }

    public RecognitionEvent(
        Guid tenantId,
        Guid cameraId,
        string plateText,
        string normalizedPlateText,
        double confidence,
        DateTime observedAt,
        string providerEventId)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CameraId = cameraId;
        PlateText = plateText;
        NormalizedPlateText = normalizedPlateText;
        Confidence = confidence;
        ObservedAt = observedAt;
        ReceivedAt = DateTime.UtcNow;
        ProviderEventId = providerEventId;
        Status = RecognitionStatus.Detected;
        CreatedAt = DateTime.UtcNow;
    }
}

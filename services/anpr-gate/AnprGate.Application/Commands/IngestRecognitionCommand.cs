using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AnprGate.Domain.Entities;
using AnprGate.Infrastructure.Persistence;

namespace AnprGate.Application.Commands;

public class IngestRecognitionCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid CameraId { get; set; }
    public string PlateText { get; set; }
    public double Confidence { get; set; }
    public DateTime ObservedAt { get; set; }
    public string ProviderEventId { get; set; }
    // Note: Evidence bytes (image) would typically be passed/uploaded separately to S3,
    // and we'd store the metadata/hash. For simplicity, we just process the plate string.
}

public class IngestRecognitionCommandHandler : IRequestHandler<IngestRecognitionCommand, Guid>
{
    private readonly AnprDbContext _context;

    public IngestRecognitionCommandHandler(AnprDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(IngestRecognitionCommand request, CancellationToken cancellationToken)
    {
        // IPlateNormalizationService logic would go here.
        var normalizedPlate = request.PlateText.ToUpper().Replace(" ", "").Replace("-", "");

        var recognitionEvent = new RecognitionEvent(
            request.TenantId,
            request.CameraId,
            request.PlateText,
            normalizedPlate,
            request.Confidence,
            request.ObservedAt,
            request.ProviderEventId);
            
        _context.RecognitionEvents.Add(recognitionEvent);
        
        // Triggers the GateAuthorizationPolicy Engine via Outbox / Integration Events internally.
        
        await _context.SaveChangesAsync(cancellationToken);

        return recognitionEvent.Id;
    }
}

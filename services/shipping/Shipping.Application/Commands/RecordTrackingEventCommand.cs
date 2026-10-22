using MediatR;
using FluentValidation;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;

namespace Shipping.Application.Commands;

// ─────────────────────────────────────────────────────────────────────────────
// Command
// ─────────────────────────────────────────────────────────────────────────────

public class RecordTrackingEventCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid ShipmentId { get; set; }
    public TrackingEventType EventType { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? VehicleIdentifier { get; set; }
    public string? CarrierEventCode { get; set; }
    public Guid UserId { get; set; }
}

// ─────────────────────────────────────────────────────────────────────────────
// Validator
// ─────────────────────────────────────────────────────────────────────────────

public class RecordTrackingEventCommandValidator : AbstractValidator<RecordTrackingEventCommand>
{
    public RecordTrackingEventCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Handler
// ─────────────────────────────────────────────────────────────────────────────

public class RecordTrackingEventCommandHandler : IRequestHandler<RecordTrackingEventCommand, Guid>
{
    private readonly IShippingQueryContext _context;

    public RecordTrackingEventCommandHandler(IShippingQueryContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordTrackingEventCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _context.GetShipmentWithTrackingAsync(request.ShipmentId, request.TenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipment {request.ShipmentId} not found.");

        shipment.AddTrackingEvent(request.EventType, request.Location, request.Description, request.UserId);

        // Enrich the last tracking event with extra metadata
        var evt = shipment.TrackingEvents.Last();
        evt.Latitude = request.Latitude;
        evt.Longitude = request.Longitude;
        evt.VehicleIdentifier = request.VehicleIdentifier;
        evt.CarrierEventCode = request.CarrierEventCode;

        // Auto-transition status based on tracking event type
        AutoTransitionStatus(shipment, request.EventType, request.UserId);

        // TODO: Publish ShipmentTrackingEventRecordedEvent to outbox

        await _context.SaveChangesAsync(cancellationToken);

        return evt.Id;
    }

    private static void AutoTransitionStatus(Shipment shipment, TrackingEventType eventType, Guid userId)
    {
        var targetStatus = eventType switch
        {
            TrackingEventType.BookingConfirmed    => ShipmentStatus.Booked,
            TrackingEventType.CargoPickedUp       => ShipmentStatus.PickedUp,
            TrackingEventType.ArrivedAtOriginTerminal => ShipmentStatus.AtOriginTerminal,
            TrackingEventType.DepartedOrigin      => ShipmentStatus.Loaded,
            TrackingEventType.ArrivedAtDestination => ShipmentStatus.AtDestinationTerminal,
            TrackingEventType.Discharged          => ShipmentStatus.Discharged,
            TrackingEventType.CustomsCleared      => ShipmentStatus.CustomsCleared,
            TrackingEventType.OutForDelivery      => ShipmentStatus.OutForDelivery,
            TrackingEventType.Delivered           => ShipmentStatus.Delivered,
            TrackingEventType.HoldPlaced          => ShipmentStatus.OnHold,
            TrackingEventType.ReturnInitiated     => ShipmentStatus.Returning,
            TrackingEventType.ReturnedToOrigin    => ShipmentStatus.Returned,
            _ => (ShipmentStatus?)null
        };

        if (targetStatus.HasValue && shipment.Status != targetStatus.Value)
            shipment.TransitionStatus(targetStatus.Value, $"Auto-transitioned by tracking event: {eventType}", userId);
    }
}

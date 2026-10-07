using MediatR;
using FluentValidation;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;

namespace Shipping.Application.Commands;

// ─────────────────────────────────────────────────────────────────────────────
// Command
// ─────────────────────────────────────────────────────────────────────────────

public class ConfirmCustomsClearanceCommand : IRequest
{
    public Guid TenantId { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid CustomsCaseId { get; set; }
    public string CustomsCaseNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

// ─────────────────────────────────────────────────────────────────────────────
// Validator
// ─────────────────────────────────────────────────────────────────────────────

public class ConfirmCustomsClearanceCommandValidator : AbstractValidator<ConfirmCustomsClearanceCommand>
{
    public ConfirmCustomsClearanceCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.CustomsCaseId).NotEmpty();
        RuleFor(x => x.CustomsCaseNumber).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Handler
// ─────────────────────────────────────────────────────────────────────────────

public class ConfirmCustomsClearanceCommandHandler : IRequestHandler<ConfirmCustomsClearanceCommand>
{
    private readonly IShippingQueryContext _context;

    public ConfirmCustomsClearanceCommandHandler(IShippingQueryContext context)
    {
        _context = context;
    }

    public async Task Handle(ConfirmCustomsClearanceCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _context.GetShipmentWithTrackingAsync(request.ShipmentId, request.TenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipment {request.ShipmentId} not found.");

        if (shipment.IsCustomsCleared)
            throw new InvalidOperationException($"Shipment {request.ShipmentId} is already customs cleared.");

        shipment.CustomsCaseId = request.CustomsCaseId;
        shipment.CustomsCaseNumber = request.CustomsCaseNumber;
        shipment.IsCustomsCleared = true;
        shipment.CustomsClearedAtUtc = DateTime.UtcNow;
        shipment.UpdatedAtUtc = DateTime.UtcNow;
        shipment.UpdatedBy = request.UserId;

        shipment.TransitionStatus(
            Enums.ShipmentStatus.CustomsCleared,
            $"Customs cleared — Case: {request.CustomsCaseNumber}",
            request.UserId);

        shipment.AddTrackingEvent(
            Enums.TrackingEventType.CustomsCleared,
            "Customs Authority",
            $"Customs clearance confirmed. Case: {request.CustomsCaseNumber}",
            request.UserId);

        // TODO: Publish ShipmentCustomsClearedEvent to outbox

        await _context.SaveChangesAsync(cancellationToken);
    }
}

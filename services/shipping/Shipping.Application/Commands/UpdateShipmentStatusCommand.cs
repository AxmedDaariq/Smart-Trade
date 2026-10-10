using MediatR;
using FluentValidation;
using Shipping.Domain.Entities;
using Shipping.Domain.Enums;

namespace Shipping.Application.Commands;

// ─────────────────────────────────────────────────────────────────────────────
// Command
// ─────────────────────────────────────────────────────────────────────────────

public class UpdateShipmentStatusCommand : IRequest
{
    public Guid TenantId { get; set; }
    public Guid ShipmentId { get; set; }
    public ShipmentStatus NewStatus { get; set; }
    public string? Reason { get; set; }
    public Guid UserId { get; set; }
}

// ─────────────────────────────────────────────────────────────────────────────
// Validator
// ─────────────────────────────────────────────────────────────────────────────

public class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500).When(x => x.Reason != null);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Context Interface
// ─────────────────────────────────────────────────────────────────────────────

public interface IShippingQueryContext
{
    IQueryable<Shipment> ShipmentsQuery { get; }
    Task<Shipment?> GetShipmentWithHistoryAsync(Guid shipmentId, Guid tenantId, CancellationToken cancellationToken);
    Task<Shipment?> GetShipmentWithTrackingAsync(Guid shipmentId, Guid tenantId, CancellationToken cancellationToken);
    Task<bool> ShipmentExistsAsync(Guid shipmentId, Guid tenantId, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

// ─────────────────────────────────────────────────────────────────────────────
// Handler
// ─────────────────────────────────────────────────────────────────────────────

public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand>
{
    private readonly IShippingQueryContext _context;

    public UpdateShipmentStatusCommandHandler(IShippingQueryContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _context.GetShipmentWithHistoryAsync(request.ShipmentId, request.TenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Shipment {request.ShipmentId} not found.");

        if (shipment.Status == ShipmentStatus.Cancelled || shipment.Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException(
                $"Cannot transition a shipment in status '{shipment.Status}' to '{request.NewStatus}'.");

        shipment.TransitionStatus(request.NewStatus, request.Reason, request.UserId);

        // TODO: Publish ShipmentStatusChangedEvent to outbox

        await _context.SaveChangesAsync(cancellationToken);
    }
}

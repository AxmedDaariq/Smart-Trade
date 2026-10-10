using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Shipping.Application.Commands;
using Microsoft.EntityFrameworkCore;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Api.Controllers;

[ApiController]
[Route("api/v1/shipments/{shipmentId:guid}/tracking")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IShippingQueryContext _query;

    public TrackingController(IMediator mediator, IShippingQueryContext query)
    {
        _mediator = mediator;
        _query = query;
    }

    // ── POST /api/v1/shipments/{shipmentId}/tracking ──────────────────────────

    /// <summary>Record a new tracking event (GPS ping, milestone scan, carrier message).</summary>
    [HttpPost]
    [Authorize(Policy = "shipping.tracking.record")]
    public async Task<IActionResult> RecordTrackingEvent(
        Guid shipmentId,
        [FromBody] RecordTrackingEventCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        command.TenantId = tenantId;
        command.ShipmentId = shipmentId;

        if (User.HasClaim(c => c.Type == "sub"))
            command.UserId = Guid.Parse(User.FindFirst("sub")!.Value);

        var eventId = await _mediator.Send(command);
        return Created($"api/v1/shipments/{shipmentId}/tracking/{eventId}", new { EventId = eventId });
    }

    // ── GET /api/v1/shipments/{shipmentId}/tracking ───────────────────────────

    /// <summary>Get the full tracking timeline for a shipment, ordered chronologically.</summary>
    [HttpGet]
    [Authorize(Policy = "shipping.read")]
    public async Task<IActionResult> GetTrackingTimeline(Guid shipmentId)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        // Verify shipment belongs to tenant
        var shipmentExists = await _query.ShipmentsQuery
            .AnyAsync(s => s.Id == shipmentId && s.TenantId == tenantId);

        if (!shipmentExists)
            return NotFound(new { Message = $"Shipment {shipmentId} not found." });

        var events = await _query.ShipmentsQuery
            .Where(s => s.Id == shipmentId)
            .SelectMany(s => s.TrackingEvents)
            .OrderBy(e => e.OccurredAtUtc)
            .Select(e => new
            {
                e.Id,
                EventType = e.EventType.ToString(),
                e.Location,
                e.Description,
                e.Latitude,
                e.Longitude,
                e.VehicleIdentifier,
                e.CarrierEventCode,
                e.OccurredAtUtc
            })
            .ToListAsync();

        return Ok(events);
    }
}

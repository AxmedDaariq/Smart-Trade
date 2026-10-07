using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Shipping.Application.Commands;
using Shipping.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IShippingQueryContext _query;

    public ShipmentsController(IMediator mediator, IShippingQueryContext query)
    {
        _mediator = mediator;
        _query = query;
    }

    // ── POST /api/v1/shipments ────────────────────────────────────────────────

    /// <summary>Create a new shipment in Draft status.</summary>
    [HttpPost]
    [Authorize(Policy = "shipping.create")]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        command.TenantId = tenantId;

        if (User.HasClaim(c => c.Type == "sub"))
            command.UserId = Guid.Parse(User.FindFirst("sub")!.Value);

        var shipmentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetShipment), new { id = shipmentId }, new { ShipmentId = shipmentId });
    }

    // ── GET /api/v1/shipments/{id} ────────────────────────────────────────────

    /// <summary>Get shipment details by ID.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "shipping.read")]
    public async Task<IActionResult> GetShipment(Guid id)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        var shipment = await _query.Shipments
            .Include(s => s.Lines)
            .Include(s => s.Containers)
            .Include(s => s.Documents)
            .Where(s => s.Id == id && s.TenantId == tenantId)
            .Select(s => new
            {
                s.Id,
                s.ShipmentNumber,
                s.ExternalReference,
                s.CarrierBookingReference,
                s.MasterBillNumber,
                s.HouseBillNumber,
                s.ShipperName,
                s.ShipperReference,
                s.ConsigneeName,
                s.ConsigneeReference,
                s.CarrierCode,
                s.CarrierName,
                s.FreightForwarderCode,
                Origin = new { s.OriginCountryCode, s.OriginPortCode, s.OriginPortName },
                Destination = new { s.DestinationCountryCode, s.DestinationPortCode, s.DestinationPortName },
                TransportMode = s.TransportMode.ToString(),
                CargoType = s.CargoType.ToString(),
                Incoterm = s.Incoterm.ToString(),
                s.GrossWeightKg,
                s.NetWeightKg,
                s.VolumeM3,
                s.PackageCount,
                s.HsCode,
                s.GoodsDescription,
                s.Currency,
                s.FreightValue,
                s.InsuranceValue,
                s.DeclaredValue,
                s.EstimatedDepartureUtc,
                s.EstimatedArrivalUtc,
                s.ActualDepartureUtc,
                s.ActualArrivalUtc,
                s.DeliveredAtUtc,
                Status = s.Status.ToString(),
                s.StatusReason,
                Customs = new
                {
                    s.CustomsCaseId,
                    s.CustomsCaseNumber,
                    s.IsCustomsCleared,
                    s.CustomsClearedAtUtc
                },
                Lines = s.Lines.Select(l => new
                {
                    l.LineNumber,
                    l.Description,
                    l.HsCode,
                    l.CountryOfOrigin,
                    l.Quantity,
                    l.UnitOfMeasure,
                    l.GrossWeightKg,
                    l.TotalValue,
                    l.Currency
                }),
                Containers = s.Containers.Select(c => new
                {
                    c.ContainerNumber,
                    c.ContainerType,
                    c.SealNumber,
                    c.CargoWeightKg,
                    c.IsHazardous,
                    c.IsReeferActive
                }),
                s.CreatedAtUtc,
                s.UpdatedAtUtc
            })
            .FirstOrDefaultAsync();

        if (shipment is null)
            return NotFound(new { Message = $"Shipment {id} not found." });

        return Ok(shipment);
    }

    // ── GET /api/v1/shipments ─────────────────────────────────────────────────

    /// <summary>List shipments for the tenant with optional status filter and pagination.</summary>
    [HttpGet]
    [Authorize(Policy = "shipping.read")]
    public async Task<IActionResult> ListShipments(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        pageSize = Math.Clamp(pageSize, 1, 100);
        page = Math.Max(1, page);

        var query = _query.Shipments.Where(s => s.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<ShipmentStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(s => s.Status == parsedStatus);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                s.ShipmentNumber,
                s.ShipperReference,
                s.ConsigneeReference,
                Origin = s.OriginPortCode,
                Destination = s.DestinationPortCode,
                Status = s.Status.ToString(),
                TransportMode = s.TransportMode.ToString(),
                s.EstimatedArrivalUtc,
                s.CreatedAtUtc
            })
            .ToListAsync();

        return Ok(new
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        });
    }

    // ── PATCH /api/v1/shipments/{id}/status ──────────────────────────────────

    /// <summary>Update the status of a shipment.</summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "shipping.status.update")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateShipmentStatusCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        command.TenantId = tenantId;
        command.ShipmentId = id;

        if (User.HasClaim(c => c.Type == "sub"))
            command.UserId = Guid.Parse(User.FindFirst("sub")!.Value);

        await _mediator.Send(command);
        return NoContent();
    }

    // ── POST /api/v1/shipments/{id}/customs-clearance ─────────────────────────

    /// <summary>Confirm customs clearance for a shipment — links to a customs case.</summary>
    [HttpPost("{id:guid}/customs-clearance")]
    [Authorize(Policy = "shipping.customs.confirm")]
    public async Task<IActionResult> ConfirmCustomsClearance(
        Guid id,
        [FromBody] ConfirmCustomsClearanceCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId))
            return Forbid();

        command.TenantId = tenantId;
        command.ShipmentId = id;

        if (User.HasClaim(c => c.Type == "sub"))
            command.UserId = Guid.Parse(User.FindFirst("sub")!.Value);

        await _mediator.Send(command);
        return NoContent();
    }
}

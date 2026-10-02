using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Corridor.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Corridor.Api.Controllers;

[ApiController]
[Route("api/v1/corridor/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{tripId}/deviations/detect")]
    [Authorize(Policy = "corridor.trip.manage")]
    public async Task<IActionResult> DetectDeviation(Guid tripId, [FromBody] DetectRouteDeviationCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId)) return Forbid();
        command.TenantId = tenantId;

        command.TripId = tripId;

        var deviated = await _mediator.Send(command);
        return Ok(new { TripId = tripId, Deviated = deviated });
    }
}

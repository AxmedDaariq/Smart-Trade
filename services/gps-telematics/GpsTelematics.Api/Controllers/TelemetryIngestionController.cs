using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using GpsTelematics.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace GpsTelematics.Api.Controllers;

[ApiController]
[Route("api/v1/gps/ingest")]
public class TelemetryIngestionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelemetryIngestionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "gps.device.ingest")]
    public async Task<IActionResult> IngestTelemetry([FromBody] IngestTelemetryCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId) ||
            !Guid.TryParse(User.FindFirst("device_id")?.Value, out var deviceId)) return Forbid();
        command.TenantId = tenantId;
        command.DeviceId = deviceId;

        var positionId = await _mediator.Send(command);
        return Accepted(new { PositionId = positionId, Status = "Processed" });
    }
}

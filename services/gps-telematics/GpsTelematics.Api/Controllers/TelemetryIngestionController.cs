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
    // Note: In production, this would use a specialized scheme for Device Token / Client Certificate auth
    // rather than standard user JWTs. We simulate standard auth here for demonstration.
    [Authorize] 
    public async Task<IActionResult> IngestTelemetry([FromBody] IngestTelemetryCommand command)
    {
        // For security, TenantId and DeviceId should be extracted from the authenticated device token
        if (User.HasClaim(c => c.Type == "tenantId"))
        {
            command.TenantId = Guid.Parse(User.FindFirst("tenantId").Value);
        }
        
        if (User.HasClaim(c => c.Type == "deviceId"))
        {
            command.DeviceId = Guid.Parse(User.FindFirst("deviceId").Value);
        }

        var positionId = await _mediator.Send(command);
        return Accepted(new { PositionId = positionId, Status = "Processed" });
    }
}

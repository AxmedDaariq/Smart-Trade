using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AnprGate.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace AnprGate.Api.Controllers;

[ApiController]
[Route("api/v1/anpr/ingest")]
// Note: Normally machine-to-machine APIs use a different policy (e.g. certificate auth or custom device tokens)
[Authorize]
public class AnprIngestionController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnprIngestionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "anpr.camera.machine")]
    public async Task<IActionResult> Ingest([FromBody] IngestRecognitionCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId) ||
            !Guid.TryParse(User.FindFirst("camera_id")?.Value, out var cameraId)) return Forbid();
        command.TenantId = tenantId;
        command.CameraId = cameraId;

        var recognitionId = await _mediator.Send(command);
        return Accepted(new { RecognitionEventId = recognitionId, Status = "Processing" });
    }
}

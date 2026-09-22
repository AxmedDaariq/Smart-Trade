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
        // Determine TenantId from authenticated camera context, NEVER from payload
        if (User.HasClaim(c => c.Type == "tenantId"))
        {
            command.TenantId = Guid.Parse(User.FindFirst("tenantId").Value);
        }

        var recognitionId = await _mediator.Send(command);
        return Accepted(new { RecognitionEventId = recognitionId, Status = "Processing" });
    }
}

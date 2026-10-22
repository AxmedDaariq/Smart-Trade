using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AisVessel.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace AisVessel.Api.Controllers;

[ApiController]
[Route("api/v1/ais/berth-allocations")]
[Authorize]
public class BerthAllocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BerthAllocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "ais.berth.allocate")]
    public async Task<IActionResult> AllocateBerth([FromBody] AllocateBerthCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId)) return Forbid();
        command.TenantId = tenantId;

        var portCallId = await _mediator.Send(command);
        return Ok(new { PortCallId = portCallId, Status = "BerthAllocated" });
    }
}

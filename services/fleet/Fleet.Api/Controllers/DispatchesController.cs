using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Fleet.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Fleet.Api.Controllers;

[ApiController]
[Route("api/v1/dispatches")]
[Authorize]
public class DispatchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DispatchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "fleet.dispatch.create")]
    public async Task<IActionResult> CreateDispatch([FromBody] CreateDispatchCommand command)
    {
        if (User.HasClaim(c => c.Type == "tenantId"))
        {
            command.TenantId = Guid.Parse(User.FindFirst("tenantId").Value);
        }

        var dispatchId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetDispatch), new { id = dispatchId }, new { DispatchId = dispatchId });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "fleet.dispatch.read")]
    public async Task<IActionResult> GetDispatch(Guid id)
    {
        return Ok(new { Id = id, Status = "Draft" });
    }
}

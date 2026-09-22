using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Ectn.Application.Commands;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Requires JWT from Phase 02
public class EctnController : ControllerBase
{
    private readonly IMediator _mediator;

    public EctnController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "ectn.create")] // Requires explicit permission
    public async Task<IActionResult> Create([FromBody] CreateEctnCommand command)
    {
        // Security Check: Ensure client cannot inject a different TenantId
        // The TenantId should be extracted from the authenticated user's JWT claims
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            command.TenantId = tenantId;
        }
        else
        {
            return Forbid("Tenant ID is missing from the authentication context.");
        }

        var actorIdClaim = User.FindFirst("sub")?.Value;
        if (Guid.TryParse(actorIdClaim, out var actorId))
        {
            command.CreatedBy = actorId;
        }

        var ectnId = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = ectnId }, new { Id = ectnId });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ectn.read")]
    public async Task<IActionResult> Get(Guid id)
    {
        // Implement GetQuery
        return Ok(new { Id = id, Status = "Draft" });
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "ectn.submit")]
    public async Task<IActionResult> Submit(Guid id)
    {
        // Execute SubmitEctnCommand
        return Ok(new { Message = "ECTN submitted successfully." });
    }

    [HttpPost("{id}/approve")]
    [Authorize(Policy = "ectn.approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        // Execute ApproveEctnCommand
        return Ok(new { Message = "ECTN approved successfully." });
    }

    [HttpPost("{id}/issue")]
    [Authorize(Policy = "ectn.issue")]
    public async Task<IActionResult> Issue(Guid id)
    {
        // Execute IssueEctnCommand
        return Ok(new { Message = "ECTN issued successfully." });
    }
}

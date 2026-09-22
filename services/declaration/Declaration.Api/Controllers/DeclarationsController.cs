using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Declaration.Application.Commands;

namespace Declaration.Api.Controllers;

[ApiController]
[Route("api/v1/declarations")]
public class DeclarationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeclarationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "declaration.create")]
    public async Task<IActionResult> Create([FromBody] CreateDeclarationCommand command)
    {
        // Enforce tenant isolation
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            command.TenantId = tenantId;
        }
        else
        {
            return Forbid("DECLARATION_TENANT_ACCESS_DENIED");
        }

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = result }, result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "declaration.read")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Ok(new { Id = id });
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "declaration.submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitDeclarationCommand command)
    {
        var tenantIdClaim = User.FindFirst("TenantId")?.Value;
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            command.TenantId = tenantId;
            command.DeclarationId = id;
        }
        else
        {
            return Forbid("DECLARATION_TENANT_ACCESS_DENIED");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("{id}/review/start")]
    [Authorize(Policy = "declaration.review")]
    public async Task<IActionResult> StartReview(Guid id)
    {
        return Ok();
    }
    
    [HttpPost("{id}/accept")]
    [Authorize(Policy = "declaration.accept")]
    public async Task<IActionResult> Accept(Guid id, [FromBody] AcceptDeclarationCommand command)
    {
        command.DeclarationId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Policy = "declaration.reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDeclarationCommand command)
    {
        command.DeclarationId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

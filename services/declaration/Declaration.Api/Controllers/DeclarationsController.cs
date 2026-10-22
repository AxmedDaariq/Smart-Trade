using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Declaration.Application.Commands;

namespace Declaration.Api.Controllers;

[ApiController]
[Route("api/v1/declarations")]
// A01: deny-by-default at controller level; individual actions add their permission policies.
[Authorize]
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
        if (!TryGetActor(out var tenantId, out var userId)) return Forbid();
        command.TenantId = tenantId;
        command.CreatedBy = userId;

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = result }, result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "declaration.read")]
    public async Task<IActionResult> Get(Guid id)
    {
        return Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Declaration lookup is not implemented.");
    }

    [HttpPost("{id}/submit")]
    [Authorize(Policy = "declaration.submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitDeclarationCommand command)
    {
        if (!TryGetActor(out var tenantId, out _)) return Forbid();
        command.TenantId = tenantId;
        command.DeclarationId = id;

        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("{id}/review/start")]
    [Authorize(Policy = "declaration.review")]
    public async Task<IActionResult> StartReview(Guid id)
    {
        return Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Declaration review is not implemented.");
    }
    
    [HttpPost("{id}/accept")]
    [Authorize(Policy = "declaration.accept")]
    public async Task<IActionResult> Accept(Guid id, [FromBody] AcceptDeclarationCommand command)
    {
        if (!TryGetActor(out var tenantId, out var userId)) return Forbid();
        command.TenantId = tenantId;
        command.DeclarationId = id;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Policy = "declaration.reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectDeclarationCommand command)
    {
        if (!TryGetActor(out var tenantId, out var userId)) return Forbid();
        command.TenantId = tenantId;
        command.DeclarationId = id;
        command.UserId = userId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    private bool TryGetActor(out Guid tenantId, out Guid userId)
    {
        var hasTenant = Guid.TryParse(User.FindFirst("tenant_id")?.Value, out tenantId);
        var hasUser = Guid.TryParse(User.FindFirst("sub")?.Value, out userId);
        return hasTenant && hasUser;
    }
}

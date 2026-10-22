using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Customs.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Customs.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomsCasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomsCasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "customs.case.create")]
    public async Task<IActionResult> CreateCase([FromBody] ReceiveDeclarationCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId)) return Forbid();
        command.TenantId = tenantId;

        // UserId from claims
        if (User.HasClaim(c => c.Type == "sub"))
        {
            command.UserId = Guid.Parse(User.FindFirst("sub").Value);
        }

        var caseId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCase), new { id = caseId }, new { CaseId = caseId });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "customs.case.read")]
    public async Task<IActionResult> GetCase(Guid id)
    {
        return Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Case lookup is not implemented.");
    }

    [HttpPost("{id}/risk-assessment")]
    [Authorize(Policy = "customs.risk.assess")]
    public async Task<IActionResult> StartRiskAssessment(Guid id)
    {
        return Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Risk assessment is not implemented.");
    }
}

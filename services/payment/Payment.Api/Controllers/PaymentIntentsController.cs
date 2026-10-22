using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Payment.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/v1/intents")]
[Authorize]
public class PaymentIntentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentIntentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "payment.intent.create")]
    public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentCommand command)
    {
        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId)) return Forbid();
        command.TenantId = tenantId;

        var intentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetIntent), new { id = intentId }, new { IntentId = intentId });
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "payment.intent.read")]
    public async Task<IActionResult> GetIntent(Guid id)
    {
        return Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Payment intent lookup is not implemented.");
    }
}

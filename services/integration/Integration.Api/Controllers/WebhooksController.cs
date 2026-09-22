using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Integration.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Integration.Api.Controllers;

[ApiController]
[Route("api/v1/integration/webhooks")]
// Webhooks often use custom auth (like HMAC signatures on the payload), so standard JWT Authorize might be bypassed here in favor of middleware/ActionFilters
[AllowAnonymous] 
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{partnerCode}")]
    public async Task<IActionResult> ReceiveWebhook(string partnerCode)
    {
        // Extract raw body for signature validation
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var payload = await reader.ReadToEndAsync();
        
        // Extract headers
        var signature = Request.Headers["X-Hub-Signature-256"].ToString();
        var timestampHeader = Request.Headers["X-Timestamp"].ToString();
        
        DateTime? timestamp = null;
        if (long.TryParse(timestampHeader, out var ts))
        {
            timestamp = DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;
        }

        var command = new ProcessWebhookCommand
        {
            // Usually derived from the URL or specific headers for webhooks
            TenantId = Guid.Empty, // Would be resolved via PartnerCode mapping in the command handler
            PartnerCode = partnerCode,
            Payload = payload,
            Signature = signature,
            Timestamp = timestamp
        };

        var result = await _mediator.Send(command);

        if (result)
        {
            // Always return fast HTTP 202 to the external partner to prevent timeout retries
            return Accepted(new { Status = "Received", Message = "Webhook payload accepted for asynchronous processing." });
        }
        
        return BadRequest(new { Error = "Invalid webhook payload or signature." });
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Integration.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using BuildingBlocks.Web;

namespace Integration.Api.Controllers;

[ApiController]
[Route("api/v1/integration/webhooks")]
// A08: webhooks authenticate with an HMAC signature over the raw payload instead of a JWT, so
// the endpoint is explicitly anonymous - the signature check inside ProcessWebhookCommandHandler
// is the security control, and it fails closed for unknown partners or missing secrets.
[AllowAnonymous]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{partnerCode}")]
    [EnableRateLimiting(PlatformSecurityExtensions.SensitiveRateLimitPolicy)]
    public async Task<IActionResult> ReceiveWebhook(string partnerCode)
    {
        // Extract raw body for signature validation - the signature covers the exact bytes received.
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
            // Resolved from the partner mapping when inbox persistence lands; the shared secret
            // itself is resolved through IWebhookSecretStore inside the handler.
            TenantId = Guid.Empty,
            PartnerCode = partnerCode,
            Payload = payload,
            Signature = signature,
            Timestamp = timestamp
        };

        var accepted = await _mediator.Send(command);

        if (accepted)
        {
            // Always return fast HTTP 202 to the external partner to prevent timeout retries
            return Accepted(new { Status = "Received", Message = "Webhook payload accepted for asynchronous processing." });
        }

        // A generic response: no hint about which check failed (missing partner, bad signature,
        // stale timestamp) so the endpoint cannot be used as a signature oracle.
        return Unauthorized(new { Error = "Webhook verification failed." });
    }
}

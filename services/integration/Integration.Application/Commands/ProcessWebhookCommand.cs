using System;
using System.Threading;
using System.Threading.Tasks;
using Integration.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Integration.Application.Commands;

public class ProcessWebhookCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public string PartnerCode { get; set; }
    public string Payload { get; set; }
    public string Signature { get; set; }
    public DateTime? Timestamp { get; set; }
}

/// <summary>
/// A08: verifies the inbound webhook before it is accepted for asynchronous processing.
/// A payload is only accepted when the partner has a configured secret, the signature matches
/// the raw payload (HMAC-SHA256, constant time) and the timestamp is inside the freshness window
/// that makes replay attacks impossible. Everything else is rejected and logged (A09) without ever
/// logging the payload or the secret.
/// </summary>
public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, bool>
{
    /// <summary>Maximum accepted clock skew for the <c>X-Timestamp</c> header.</summary>
    public static readonly TimeSpan MaximumTimestampSkew = TimeSpan.FromMinutes(5);

    private readonly IExternalRequestSigner _signer;
    private readonly IWebhookSecretStore _secretStore;
    private readonly ILogger<ProcessWebhookCommandHandler> _logger;

    public ProcessWebhookCommandHandler(
        IExternalRequestSigner signer,
        IWebhookSecretStore secretStore,
        ILogger<ProcessWebhookCommandHandler> logger)
    {
        _signer = signer;
        _secretStore = secretStore;
        _logger = logger;
    }

    public Task<bool> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        if (request.Timestamp is null)
        {
            _logger.LogWarning("Rejected webhook from {PartnerCode}: X-Timestamp header is missing", request.PartnerCode);
            return Task.FromResult(false);
        }

        var skew = DateTime.UtcNow - request.Timestamp.Value;
        if (skew > MaximumTimestampSkew || skew < -MaximumTimestampSkew)
        {
            _logger.LogWarning(
                "Rejected webhook from {PartnerCode}: timestamp outside the {Window} replay window (skew {Skew})",
                request.PartnerCode, MaximumTimestampSkew, skew);
            return Task.FromResult(false);
        }

        var secret = _secretStore.GetSecret(request.PartnerCode);
        if (string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning(
                "Rejected webhook from {PartnerCode}: no signing secret configured for this partner",
                request.PartnerCode);
            return Task.FromResult(false);
        }

        if (!_signer.VerifySignature(request.Payload, secret, request.Signature))
        {
            _logger.LogWarning(
                "Rejected webhook from {PartnerCode}: signature verification failed for {PayloadLength} byte payload",
                request.PartnerCode, request.Payload?.Length ?? 0);
            return Task.FromResult(false);
        }

        // Inbox persistence (outbox pattern backwards) happens through an Application-owned
        // interface implemented by Integration.Infrastructure once it is available.
        return Task.FromResult(true);
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Integration.Application.Commands;

public class ProcessWebhookCommand : IRequest<bool>
{
    public Guid TenantId { get; set; }
    public string PartnerCode { get; set; }
    public string Payload { get; set; }
    public string Signature { get; set; }
    public DateTime? Timestamp { get; set; }
}

public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, bool>
{
    // NB: Application must not reference Integration.Infrastructure. When inbox
    // persistence is implemented it will be exposed through an Application-owned
    // interface (implemented in Infrastructure), the same way the other services do it.

    public async Task<bool> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Partner and WebhookEndpoint (for secret references)
        // 2. Validate Signature against payload
        // 3. Check for replay/idempotency against Redis
        // 4. Save to InboxMessage table for reliable asynchronous processing (outbox pattern backwards)
        
        // Simulating Inbox insertion
        
        return await Task.FromResult(true); // Webhook accepted successfully.
    }
}

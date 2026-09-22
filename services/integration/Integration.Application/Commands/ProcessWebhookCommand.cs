using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Integration.Domain.Entities;
using Integration.Infrastructure.Persistence;

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
    private readonly IntegrationDbContext _context;
    
    // In a real application, we'd inject IExternalRequestSigner to verify the HMAC

    public ProcessWebhookCommandHandler(IntegrationDbContext context)
    {
        _context = context;
    }

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

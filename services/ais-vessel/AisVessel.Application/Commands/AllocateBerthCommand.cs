using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AisVessel.Domain.Entities;
using AisVessel.Domain.Enums;
using AisVessel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AisVessel.Application.Commands;

public class AllocateBerthCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid PortCallId { get; set; }
    public Guid BerthId { get; set; }
}

public class AllocateBerthCommandHandler : IRequestHandler<AllocateBerthCommand, Guid>
{
    private readonly AisDbContext _context;

    public AllocateBerthCommandHandler(AisDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AllocateBerthCommand request, CancellationToken cancellationToken)
    {
        var portCall = await _context.PortCalls
            .FirstOrDefaultAsync(p => p.Id == request.PortCallId && p.TenantId == request.TenantId, cancellationToken);
            
        if (portCall == null) throw new InvalidOperationException("PortCall not found.");
        
        var berth = await _context.Berths
            .FirstOrDefaultAsync(b => b.Id == request.BerthId && b.TenantId == request.TenantId, cancellationToken);
            
        if (berth == null) throw new InvalidOperationException("Berth not found.");

        if (portCall.Status != PortCallStatus.AwaitingBerth)
        {
            throw new InvalidOperationException("PortCall must be AwaitingBerth to allocate a berth.");
        }

        portCall.AdvanceState(PortCallStatus.BerthAllocated);
        
        // Example: in full implementation, we'd add the BerthAllocation entity here,
        // evaluate the IBerthAllocationService rules to ensure Length/Draft match,
        // and raise a BerthAllocatedIntegrationEvent via Outbox.
        
        await _context.SaveChangesAsync(cancellationToken);

        return portCall.Id;
    }
}

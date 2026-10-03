using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AisVessel.Application.Interfaces;
using AisVessel.Domain.Enums;

namespace AisVessel.Application.Commands;

public class AllocateBerthCommand : IRequest<Guid>
{
    public Guid TenantId { get; set; }
    public Guid PortCallId { get; set; }
    public Guid BerthId { get; set; }
}

public class AllocateBerthCommandHandler : IRequestHandler<AllocateBerthCommand, Guid>
{
    private readonly IBerthAllocationRepository _repository;

    public AllocateBerthCommandHandler(IBerthAllocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AllocateBerthCommand request, CancellationToken cancellationToken)
    {
        var portCall = await _repository.GetPortCallAsync(request.PortCallId, request.TenantId, cancellationToken);

        if (portCall == null) throw new InvalidOperationException("PortCall not found.");

        var berth = await _repository.GetBerthAsync(request.BerthId, request.TenantId, cancellationToken);

        if (berth == null) throw new InvalidOperationException("Berth not found.");

        if (portCall.Status != nameof(PortCallStatus.AwaitingBerth))
        {
            throw new InvalidOperationException("PortCall must be AwaitingBerth to allocate a berth.");
        }

        portCall.AdvanceState(PortCallStatus.BerthAllocated);

        // Example: in full implementation, we'd add the BerthAllocation entity here,
        // evaluate the IBerthAllocationService rules to ensure Length/Draft match,
        // and raise a BerthAllocatedIntegrationEvent via Outbox.

        await _repository.SaveChangesAsync(cancellationToken);

        return portCall.Id;
    }
}

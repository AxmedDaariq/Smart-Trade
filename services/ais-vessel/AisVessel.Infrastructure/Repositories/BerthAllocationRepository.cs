using System;
using System.Threading;
using System.Threading.Tasks;
using AisVessel.Application.Interfaces;
using AisVessel.Domain.Entities;
using AisVessel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AisVessel.Infrastructure.Repositories;

public class BerthAllocationRepository : IBerthAllocationRepository
{
    private readonly AisDbContext _context;

    public BerthAllocationRepository(AisDbContext context)
    {
        _context = context;
    }

    public async Task<PortCall?> GetPortCallAsync(Guid portCallId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.PortCalls
            .FirstOrDefaultAsync(p => p.Id == portCallId && p.TenantId == tenantId, cancellationToken);
    }

    public async Task<Berth?> GetBerthAsync(Guid berthId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Berths
            .FirstOrDefaultAsync(b => b.Id == berthId && b.TenantId == tenantId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

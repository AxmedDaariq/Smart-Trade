using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AisVessel.Domain.Entities;
using AisVessel.Domain.Repositories;
using AisVessel.Infrastructure.Persistence;

namespace AisVessel.Infrastructure.Repositories;

public class AisVesselRepository : IAisVesselRepository
{
    private readonly AisDbContext _context;

    public AisVesselRepository(AisDbContext context)
    {
        _context = context;
    }

    public async Task<Vessel> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Vessels.FirstOrDefaultAsync(v => v.Id == id && v.TenantId == tenantId);
    }

    public async Task<PortCall> GetPortCallByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.PortCalls.FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }

    public async Task<bool> BerthExistsAsync(Guid berthId, Guid tenantId)
    {
        return await _context.Berths.AnyAsync(b => b.Id == berthId && b.TenantId == tenantId);
    }

    public async Task AddAsync(object entity)
    {
        await _context.AddAsync(entity);
    }

    public async Task UpdateAsync(object entity)
    {
        _context.Update(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

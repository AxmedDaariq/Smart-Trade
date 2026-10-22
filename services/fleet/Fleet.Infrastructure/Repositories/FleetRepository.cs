using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fleet.Domain.Entities;
using Fleet.Domain.Repositories;
using Fleet.Infrastructure.Persistence;

namespace Fleet.Infrastructure.Repositories;

public class FleetRepository : IFleetRepository
{
    private readonly FleetDbContext _context;

    public FleetRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<DispatchOrder> GetDispatchByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.DispatchOrders.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);
    }

    public async Task AddAsync(DispatchOrder dispatch)
    {
        await _context.DispatchOrders.AddAsync(dispatch);
    }

    public async Task UpdateAsync(DispatchOrder dispatch)
    {
        _context.DispatchOrders.Update(dispatch);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

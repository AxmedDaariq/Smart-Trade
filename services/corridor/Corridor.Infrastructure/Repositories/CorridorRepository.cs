using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Corridor.Domain.Entities;
using Corridor.Domain.Repositories;
using Corridor.Infrastructure.Persistence;

namespace Corridor.Infrastructure.Repositories;

public class CorridorRepository : ICorridorRepository
{
    private readonly CorridorDbContext _context;

    public CorridorRepository(CorridorDbContext context)
    {
        _context = context;
    }

    public async Task<Trip> GetTripByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Trips.FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);
    }

    public async Task<RouteVersion> GetRouteVersionByIdAsync(Guid id)
    {
        return await _context.RouteVersions.FindAsync(id);
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

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GpsTelematics.Domain.Entities;
using GpsTelematics.Domain.Repositories;
using GpsTelematics.Infrastructure.Persistence;

namespace GpsTelematics.Infrastructure.Repositories;

public class GpsRepository : IGpsRepository
{
    private readonly GpsDbContext _context;

    public GpsRepository(GpsDbContext context)
    {
        _context = context;
    }

    public async Task<GpsPosition> GetPositionByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.GpsPositions.FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
    }


    public async Task AddPositionAsync(GpsPosition position)
    {
        await _context.GpsPositions.AddAsync(position);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

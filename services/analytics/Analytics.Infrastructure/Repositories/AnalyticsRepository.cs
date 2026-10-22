using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Analytics.Infrastructure.Persistence;

namespace Analytics.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AnalyticsDbContext _context;

    public AnalyticsRepository(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<KpiDefinition> GetKpiByIdAsync(Guid id)
    {
        return await _context.KpiDefinitions.FindAsync(id);
    }

    public async Task<bool> FactExistsAsync(Guid eventId)
    {
        return await _context.FactTripMilestones.AnyAsync(f => f.SourceEventId == eventId);
    }

    public async Task AddFactAsync(FactTripMilestone fact)
    {
        await _context.FactTripMilestones.AddAsync(fact);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

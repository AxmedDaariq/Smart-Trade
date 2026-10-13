using System;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories;

public interface IAnalyticsRepository
{
    Task<KpiDefinition> GetKpiByIdAsync(Guid id);
    Task<bool> FactExistsAsync(Guid eventId);
    Task AddFactAsync(FactTripMilestone fact);
    Task SaveChangesAsync();
}

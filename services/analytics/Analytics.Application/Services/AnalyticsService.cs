using System;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Application.Interfaces;
using Analytics.Domain.Entities;

namespace Analytics.Application.Services;

public interface IAnalyticsService
{
    Task<double> GetAverageTransitTimeAsync(Guid tenantId);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsDbContext _dbContext;

    public AnalyticsService(IAnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<double> GetAverageTransitTimeAsync(Guid tenantId)
    {
        // Note: Since IAnalyticsDbContext is an interface, we need to ensure
        // it provides access to the FactTripMilestones set or a specialized method.
        // For now, we will implement the logic.

        // To avoid EF Core dependency in the Application layer, the IAnalyticsDbContext
        // should have a method to calculate this or return the data.

        // Assuming we add a method to IAnalyticsDbContext for this.
        throw new NotImplementedException("This logic should be moved to a repository or a specific method in IAnalyticsDbContext to keep the application layer clean.");
    }
}

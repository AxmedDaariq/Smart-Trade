using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Analytics.Infrastructure.Persistence;

namespace Analytics.IntegrationTests;

public class AnalyticsApiFactory : BaseIntegrationTestFactory<Program>
{
    public AnalyticsApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<AnalyticsDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

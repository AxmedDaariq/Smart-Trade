using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Fleet.Infrastructure.Persistence;

namespace Fleet.IntegrationTests;

public class FleetApiFactory : BaseIntegrationTestFactory<Program>
{
    public FleetApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<FleetDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

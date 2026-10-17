using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Corridor.Infrastructure.Persistence;

namespace Corridor.IntegrationTests;

public class CorridorApiFactory : BaseIntegrationTestFactory<Program>
{
    public CorridorApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<CorridorDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using AnprGate.Infrastructure.Persistence;

namespace AnprGate.IntegrationTests;

public class AnprGateApiFactory : BaseIntegrationTestFactory<Program>
{
    public AnprGateApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<AnprDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

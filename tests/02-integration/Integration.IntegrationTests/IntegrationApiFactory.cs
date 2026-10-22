using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Integration.Infrastructure.Persistence;

namespace Integration.IntegrationTests;

public class IntegrationApiFactory : BaseIntegrationTestFactory<Program>
{
    public IntegrationApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<IntegrationDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

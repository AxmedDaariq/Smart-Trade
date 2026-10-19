using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using AisVessel.Infrastructure.Persistence;

namespace AisVessel.IntegrationTests;

public class AisVesselApiFactory : BaseIntegrationTestFactory<Program>
{
    public AisVesselApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<AisDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

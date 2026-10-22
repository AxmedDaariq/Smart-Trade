using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using GpsTelematics.Infrastructure.Persistence;

namespace GpsTelematics.IntegrationTests;

public class GpsTelematicsApiFactory : BaseIntegrationTestFactory<Program>
{
    public GpsTelematicsApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<GpsDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

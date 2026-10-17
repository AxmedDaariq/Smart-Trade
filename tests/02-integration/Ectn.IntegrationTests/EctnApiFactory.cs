using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Ectn.Infrastructure.Persistence;

namespace Ectn.IntegrationTests;

public class EctnApiFactory : BaseIntegrationTestFactory<Program>
{
    public EctnApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<EctnDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

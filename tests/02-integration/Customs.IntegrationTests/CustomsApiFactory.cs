using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Customs.Infrastructure.Persistence;

namespace Customs.IntegrationTests;

public class CustomsApiFactory : BaseIntegrationTestFactory<Program>
{
    public CustomsApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<CustomsDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

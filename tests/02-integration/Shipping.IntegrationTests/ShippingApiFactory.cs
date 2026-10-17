using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Shipping.Infrastructure.Persistence;

namespace Shipping.IntegrationTests;

public class ShippingApiFactory : BaseIntegrationTestFactory<Program>
{
    public ShippingApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<ShippingDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

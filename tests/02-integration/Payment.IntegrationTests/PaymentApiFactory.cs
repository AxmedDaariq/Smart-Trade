using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Payment.Infrastructure.Persistence;

namespace Payment.IntegrationTests;

public class PaymentApiFactory : BaseIntegrationTestFactory<Program>
{
    public PaymentApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<PaymentDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

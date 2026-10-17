using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Identity.Infrastructure.Persistence;

namespace Identity.IntegrationTests;

public class IdentityApiFactory : BaseIntegrationTestFactory<Program>
{
    public IdentityApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Identity specific overrides can go here
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<IdentityDbContext>();
            await db.Database.MigrateAsync();
        });
    }
}

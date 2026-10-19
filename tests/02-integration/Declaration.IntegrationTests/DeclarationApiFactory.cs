using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Shared.IntegrationTests;
using Declaration.Infrastructure.Persistence;

namespace Declaration.IntegrationTests;

public class DeclarationApiFactory : BaseIntegrationTestFactory<Program>
{
    public DeclarationApiFactory(ContainerFixture containerFixture) : base(containerFixture)
    {
    }

    public async Task MigrateDatabaseAsync()
    {
        await MigrateDatabaseAsync(async sp =>
        {
            var db = sp.GetRequiredService<DeclarationDbContext>();
            await db.Database.EnsureCreatedAsync();
        });
    }
}

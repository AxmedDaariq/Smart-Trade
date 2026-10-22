using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.IntegrationTests;

public abstract class BaseIntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected readonly ContainerFixture _containerFixture;

    protected BaseIntegrationTestFactory(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _containerFixture.GetPostgresConnectionString() },
                { "ConnectionStrings:ShippingDb", _containerFixture.GetPostgresConnectionString() },
                { "Redis:ConnectionString", _containerFixture.GetRedisConnectionString() },
                { "Jwt:Secret", "IntegrationTestingSuperSecretKeyWithAtLeast32BytesLength!" },
                { "Jwt:Issuer", "smart-trade-identity" },
                { "Jwt:Audience", "smart-trade-platform" }
            });
        });
    }

    public async Task MigrateDatabaseAsync(Func<IServiceProvider, Task> migrateAction)
    {
        using var scope = Services.CreateScope();
        await migrateAction(scope.ServiceProvider);
    }
}

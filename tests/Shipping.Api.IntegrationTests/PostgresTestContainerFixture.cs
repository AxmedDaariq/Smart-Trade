using Microsoft.EntityFrameworkCore;
using Shipping.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace Shipping.Api.IntegrationTests;

/// <summary>
/// Shared collection fixture: one PostgreSQL container per test collection,
/// started once and reused by every integration test class. The Shipping
/// schema is created up-front with EnsureCreated (the service does not run
/// migrations at startup).
/// </summary>
public class PostgresTestContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("shipping_it")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgreSql.StartAsync();
        ConnectionString = _postgreSql.GetConnectionString();

        await using var ctx = new ShippingDbContext(
            new DbContextOptionsBuilder<ShippingDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);

        await ctx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await _postgreSql.DisposeAsync();
}

[CollectionDefinition("shipping-postgres")]
public class ShippingPostgresCollection : ICollectionFixture<PostgresTestContainerFixture>;

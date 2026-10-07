using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Identity.Api.IntegrationTests;

/// <summary>
/// Shared collection fixture: one PostgreSQL container per test collection,
/// started once and reused by every integration test class. The Identity
/// schema is created up-front with EnsureCreated (the service does not run
/// migrations at startup).
/// </summary>
public class IdentityPostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("identity_it")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgreSql.StartAsync();
        ConnectionString = _postgreSql.GetConnectionString();

        await using var ctx = new IdentityDbContext(
            new DbContextOptionsBuilder<IdentityDbContext>()
                .UseNpgsql(ConnectionString)
                .Options);

        await ctx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await _postgreSql.DisposeAsync();
}

[CollectionDefinition("identity-postgres")]
public class IdentityPostgresCollection : ICollectionFixture<IdentityPostgresFixture>;

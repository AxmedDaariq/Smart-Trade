using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace Shared.IntegrationTests;

public class ContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("integration_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    public RedisContainer RedisContainer { get; } = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    public string GetPostgresConnectionString() => PostgresContainer.GetConnectionString();
    public string GetRedisConnectionString() => RedisContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(PostgresContainer.StartAsync(), RedisContainer.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(PostgresContainer.StopAsync(), RedisContainer.StopAsync());
    }
}

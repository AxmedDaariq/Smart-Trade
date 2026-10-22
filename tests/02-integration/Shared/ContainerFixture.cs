using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace Shared.IntegrationTests;

public class ContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder()
        .WithImage("postgis/postgis:15-3.3")
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
        var pgConn = GetPostgresConnectionString();
        var redisConn = GetRedisConnectionString();
        const string jwtSecret = "IntegrationTestingSuperSecretKeyWithAtLeast32BytesLength!";
        const string jwtIssuer = "smart-trade-identity";
        const string jwtAudience = "smart-trade-platform";

        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", pgConn);
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", pgConn);
        Environment.SetEnvironmentVariable("Redis__ConnectionString", redisConn);
        Environment.SetEnvironmentVariable("Redis:ConnectionString", redisConn);
        Environment.SetEnvironmentVariable("Jwt__Secret", jwtSecret);
        Environment.SetEnvironmentVariable("Jwt:Secret", jwtSecret);
        Environment.SetEnvironmentVariable("Jwt__Issuer", jwtIssuer);
        Environment.SetEnvironmentVariable("Jwt:Issuer", jwtIssuer);
        Environment.SetEnvironmentVariable("Jwt__Audience", jwtAudience);
        Environment.SetEnvironmentVariable("Jwt:Audience", jwtAudience);
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(PostgresContainer.StopAsync(), RedisContainer.StopAsync());
    }
}

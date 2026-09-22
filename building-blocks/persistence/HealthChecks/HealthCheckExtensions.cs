using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Persistence.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddDatabaseHealthChecks(
        this IServiceCollection services, 
        string postgresConnectionString, 
        string mongoConnectionString, 
        string redisConnectionString)
    {
        var builder = services.AddHealthChecks();

        if (!string.IsNullOrEmpty(postgresConnectionString))
        {
            // Requires: AspNetCore.HealthChecks.Npgsql
            builder.AddNpgSql(
                postgresConnectionString,
                name: "postgresql",
                tags: new[] { "db", "sql", "ready" });
        }

        if (!string.IsNullOrEmpty(mongoConnectionString))
        {
            // Requires: AspNetCore.HealthChecks.MongoDb
            builder.AddMongoDb(
                mongoConnectionString,
                name: "mongodb",
                tags: new[] { "db", "nosql", "ready" });
        }

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            // Requires: AspNetCore.HealthChecks.Redis
            builder.AddRedis(
                redisConnectionString,
                name: "redis",
                tags: new[] { "cache", "ready" });
        }

        return builder;
    }
}

namespace BuildingBlocks.Persistence.Options;

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelaySeconds { get; set; } = 30;
    public int CommandTimeoutSeconds { get; set; } = 30;
}

public class MongoOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public class RedisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}

namespace BuildingBlocks.Redis;

public interface IApplicationCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    // Explicit cache-aside abstraction
    Task<T> GetOrSetAsync<T>(
        string key, 
        Func<CancellationToken, Task<T>> factory, 
        TimeSpan? ttl = null, 
        CancellationToken cancellationToken = default);
}

namespace BuildingBlocks.Redis;

public interface IIdempotencyStore
{
    // Try to lock the idempotency key (NX)
    Task<bool> TryAcquireAsync(
        string idempotencyKey, 
        string operationName, 
        TimeSpan expiration, 
        CancellationToken cancellationToken = default);
        
    // Save the result so subsequent identical requests get the same result
    Task SaveResultAsync<T>(
        string idempotencyKey, 
        string operationName, 
        T result, 
        TimeSpan retentionPeriod, 
        CancellationToken cancellationToken = default);
        
    // Fetch a previously saved result
    Task<T?> GetResultAsync<T>(
        string idempotencyKey, 
        string operationName, 
        CancellationToken cancellationToken = default);
}

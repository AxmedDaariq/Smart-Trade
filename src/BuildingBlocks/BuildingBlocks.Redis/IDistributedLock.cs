namespace BuildingBlocks.Redis;

public interface IDistributedLock : IAsyncDisposable
{
    string LockId { get; }
    string Resource { get; }
    bool IsAcquired { get; }
    
    Task ReleaseAsync(CancellationToken cancellationToken = default);
}

public interface IDistributedLockFactory
{
    Task<IDistributedLock> AcquireLockAsync(
        string resource, 
        TimeSpan expiration, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default);
}

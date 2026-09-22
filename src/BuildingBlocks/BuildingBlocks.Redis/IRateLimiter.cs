namespace BuildingBlocks.Redis;

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public long Remaining { get; set; }
    public TimeSpan? RetryAfter { get; set; }
}

public interface IRateLimiter
{
    Task<RateLimitResult> CheckLimitAsync(
        string resourceKey, 
        int maxRequests, 
        TimeSpan window, 
        CancellationToken cancellationToken = default);
}

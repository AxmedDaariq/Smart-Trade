namespace SmartTrade.BuildingBlocks.Persistence;

public static class PaginationConstants
{
    public const int DefaultPageSize = 50;
    public const int MaxPageSize = 1000;
}

public class KeysetPaginationRequest
{
    private int _limit = PaginationConstants.DefaultPageSize;

    public int Limit
    {
        get => _limit;
        set => _limit = (value > PaginationConstants.MaxPageSize) ? PaginationConstants.MaxPageSize : value;
    }

    public DateTimeOffset? CursorCreatedAt { get; set; }
    public Guid? CursorId { get; set; }
}

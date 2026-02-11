namespace Application.Common;

public sealed class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

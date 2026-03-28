using System.Linq;

namespace aemarcoCommons.Extensions;

public interface IPaging
{
    public int? Skip { get; }
    public int? Take { get; }

    public int? Page { get; }
    public int? PageSize { get; }

}

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IPaging? paging)
    {
        if (paging is { Skip: >= 0 and var skip, Take: > 0 and var take })
        {
            query = query
                .Skip(skip)
                .Take(take);
        }
        else if (paging is { Page: > 0 and var page, PageSize: > 0 and var pageSize })
        {
            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
        return query;
    }
}



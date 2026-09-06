using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace appOne.Helpers;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public static void SetPageUrls<T>(this PagedResult<T> result, HttpRequest request)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";
        var query = QueryHelpers.ParseQuery(request.QueryString.Value ?? string.Empty)
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        if (result.HasNextPage)
        {
            query["page"] = (result.PageNumber + 1).ToString();
            query["pageSize"] = result.PageSize.ToString();
            result.NextPageUrl = QueryHelpers.AddQueryString(baseUrl, query);
        }

        if (result.HasPreviousPage)
        {
            query["page"] = (result.PageNumber - 1).ToString();
            query["pageSize"] = result.PageSize.ToString();
            result.PreviousPageUrl = QueryHelpers.AddQueryString(baseUrl, query);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using appOne.Helpers;

namespace appOne.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(T data, string message = "Berhasil")
        => Ok(ApiResponse<T>.Ok(data, message));

    // ⬇️ overload baru, khusus kalau data-nya PagedResult<T>
    protected IActionResult SuccessResponse<T>(PagedResult<T> pagedResult, string message = "Berhasil")
    {
        var meta = new PagedMeta
        {
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount,
            TotalPages = pagedResult.TotalPages,
            HasPreviousPage = pagedResult.HasPreviousPage,
            HasNextPage = pagedResult.HasNextPage,
            NextPageUrl = pagedResult.NextPageUrl,
            PreviousPageUrl = pagedResult.PreviousPageUrl
        };

        return Ok(ApiResponse<List<T>>.Ok(pagedResult.Items, message, meta));
    }

    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data, string message = "Data berhasil disimpan")
        => CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message));

    protected IActionResult FailResponse(string message)
        => BadRequest(ApiResponse<object>.Fail(message));

    protected IActionResult NotFoundResponse(string message = "Data tidak ditemukan")
        => NotFound(ApiResponse<object>.Fail(message));
}
using Microsoft.AspNetCore.Mvc;
using appOne.Helpers;

namespace appOne.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(T data, string message = "Berhasil")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data, string message = "Data berhasil disimpan")
        => CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message));

    protected IActionResult FailResponse(string message)
        => BadRequest(ApiResponse<object>.Fail(message));

    protected IActionResult NotFoundResponse(string message = "Data tidak ditemukan")
        => NotFound(ApiResponse<object>.Fail(message));
}
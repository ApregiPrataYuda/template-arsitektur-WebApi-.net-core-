using Microsoft.AspNetCore.Mvc;
using appOne.DTOs;
using appOne.Services;
using Microsoft.AspNetCore.Authorization;
using appOne.Helpers;

namespace appOne.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccessMenusController : BaseApiController
{
    private readonly IAccessMenuService _service;

    public AccessMenusController(IAccessMenuService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        result.SetPageUrls(Request);
        return SuccessResponse(result, "Data berhasil diambil");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null
            ? NotFoundResponse()
            : SuccessResponse(result, "Data berhasil diambil");
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccessMenuCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedResponse(nameof(GetById), new { id = result.IdAccessMenu }, result, "Akses menu berhasil disimpan");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccessMenuUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null
            ? NotFoundResponse()
            : SuccessResponse(result, "Akses menu berhasil diupdate");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success
            ? SuccessResponse<object>(new { }, "Akses menu berhasil dihapus")
            : NotFoundResponse();
    }
}
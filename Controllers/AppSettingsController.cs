using Microsoft.AspNetCore.Mvc;
using appOne.DTOs;
using appOne.Services;
using appOne.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace appOne.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppSettingsController : BaseApiController
{
    private readonly IAppSettingService _service;

    public AppSettingsController(IAppSettingService service)
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
    public async Task<IActionResult> Create(AppSettingCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result, "App setting berhasil disimpan");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AppSettingUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null
            ? NotFoundResponse()
            : SuccessResponse(result, "App setting berhasil diupdate");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success
            ? SuccessResponse<object>(new { }, "App setting berhasil dihapus")
            : NotFoundResponse();
    }

    [HttpPost("{id}/logo")]
    public async Task<IActionResult> UploadLogo(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return FailResponse("File logo wajib diisi.");

        var path = await _service.UpdateLogoAsync(id, file);
        return path == null
            ? NotFoundResponse()
            : SuccessResponse(new { logo = path }, "Logo berhasil diupload");
    }

    [HttpPost("{id}/logo-small")]
    public async Task<IActionResult> UploadLogoSmall(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return FailResponse("File logo kecil wajib diisi.");

        var path = await _service.UpdateLogoSmallAsync(id, file);
        return path == null
            ? NotFoundResponse()
            : SuccessResponse(new { logoSmall = path }, "Logo kecil berhasil diupload");
    }

    [HttpPost("{id}/favicon")]
    public async Task<IActionResult> UploadFavicon(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return FailResponse("File favicon wajib diisi.");

        var path = await _service.UpdateFaviconAsync(id, file);
        return path == null
            ? NotFoundResponse()
            : SuccessResponse(new { favicon = path }, "Favicon berhasil diupload");
    }
}
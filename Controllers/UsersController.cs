using Microsoft.AspNetCore.Mvc;
using appOne.DTOs;
using appOne.Services;
using Microsoft.AspNetCore.Authorization;
using appOne.Helpers;            


namespace appOne.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
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
    public async Task<IActionResult> Create(UserCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedResponse(nameof(GetById), new { id = result.IdUser }, result, "User berhasil disimpan");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result == null
            ? NotFoundResponse()
            : SuccessResponse(result, "User berhasil diupdate");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success
            ? SuccessResponse<object>(new { }, "User berhasil dihapus")
            : NotFoundResponse();
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return FailResponse("File gambar wajib diisi.");

        var path = await _service.UpdateImageAsync(id, file);
        return path == null
            ? NotFoundResponse()
            : SuccessResponse(new { image = path }, "Foto berhasil diupload");
    }
}
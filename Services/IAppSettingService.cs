using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IAppSettingService
{
    Task<PagedResult<AppSettingResponseDto>> GetAllAsync(int page, int pageSize);
    Task<AppSettingResponseDto?> GetByIdAsync(int id);
    Task<AppSettingResponseDto> CreateAsync(AppSettingCreateDto dto);
    Task<AppSettingResponseDto?> UpdateAsync(int id, AppSettingUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<string?> UpdateLogoAsync(int id, IFormFile file);
    Task<string?> UpdateLogoSmallAsync(int id, IFormFile file);
    Task<string?> UpdateFaviconAsync(int id, IFormFile file);
}
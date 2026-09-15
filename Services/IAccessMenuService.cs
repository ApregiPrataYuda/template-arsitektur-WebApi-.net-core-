using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IAccessMenuService
{
    Task<PagedResult<AccessMenuResponseDto>> GetAllAsync(int page, int pageSize);
    Task<AccessMenuResponseDto?> GetByIdAsync(int id);
    Task<AccessMenuResponseDto> CreateAsync(AccessMenuCreateDto dto);
    Task<AccessMenuResponseDto?> UpdateAsync(int id, AccessMenuUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IAccessSubMenuService
{
    Task<PagedResult<AccessSubMenuResponseDto>> GetAllAsync(int page, int pageSize);
    Task<AccessSubMenuResponseDto?> GetByIdAsync(int id);
    Task<AccessSubMenuResponseDto> CreateAsync(AccessSubMenuCreateDto dto);
    Task<AccessSubMenuResponseDto?> UpdateAsync(int id, AccessSubMenuUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
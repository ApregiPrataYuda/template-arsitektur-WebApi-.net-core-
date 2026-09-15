using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface ISubMenuService
{
    Task<PagedResult<SubMenuResponseDto>> GetAllAsync(int page, int pageSize);
    Task<SubMenuResponseDto?> GetByIdAsync(int id);
    Task<SubMenuResponseDto> CreateAsync(SubMenuCreateDto dto);
    Task<SubMenuResponseDto?> UpdateAsync(int id, SubMenuUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
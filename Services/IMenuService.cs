using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IMenuService
{
    Task<PagedResult<MenuResponseDto>> GetAllAsync(int page, int pageSize);
    Task<MenuResponseDto?> GetByIdAsync(int id);
    Task<MenuResponseDto> CreateAsync(MenuCreateDto dto);
    Task<MenuResponseDto?> UpdateAsync(int id, MenuUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
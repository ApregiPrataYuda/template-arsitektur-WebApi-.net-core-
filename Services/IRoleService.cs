using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IRoleService
{
    Task<PagedResult<RoleResponseDto>> GetAllAsync(int page, int pageSize);
    Task<RoleResponseDto?> GetByIdAsync(int id);
    Task<RoleResponseDto> CreateAsync(RoleCreateDto dto);
    Task<RoleResponseDto?> UpdateAsync(int id, RoleUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
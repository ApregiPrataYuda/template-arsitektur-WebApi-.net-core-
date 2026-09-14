using appOne.DTOs;
using appOne.Helpers;

namespace appOne.Services;

public interface IUserService
{
    Task<PagedResult<UserResponseDto>> GetAllAsync(int page, int pageSize);
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<UserResponseDto> CreateAsync(UserCreateDto dto);
    Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<string?> UpdateImageAsync(int id, IFormFile file);
}
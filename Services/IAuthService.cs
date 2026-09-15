using appOne.DTOs;

namespace appOne.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<ProfileResponseDto?> GetProfileAsync(int userId);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);   
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);          
}
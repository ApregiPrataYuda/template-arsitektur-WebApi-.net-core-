using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using appOne.Data;
using appOne.DTOs;
using appOne.Helpers;
using appOne.Models;

namespace appOne.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                (u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail)
                && u.DeletedAt == null);

        if (user == null) return null;
        if (!user.IsActive) throw new UnauthorizedAccessException("Akun tidak aktif, hubungi administrator.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

        return await BuildResponseAsync(user, user.Role?.RoleName);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var exists = await _context.Users
            .AnyAsync(u => (u.Username == dto.Username || u.Email == dto.Email) && u.DeletedAt == null);

        if (exists)
            throw new DuplicateDataException("Username atau email sudah terdaftar.");

        var roleExists = await _context.Roles.AnyAsync(r => r.IdRole == dto.RoleId && r.DeletedAt == null);
        if (!roleExists)
            throw new DuplicateDataException($"Role dengan id {dto.RoleId} tidak ditemukan.");

        var user = new User
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = dto.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FindAsync(dto.RoleId);
        return await BuildResponseAsync(user, role?.RoleName);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        // Refresh token nggak ketemu, sudah di-revoke, atau sudah kadaluarsa
        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            return null;

        if (!storedToken.User.IsActive)
            throw new UnauthorizedAccessException("Akun tidak aktif, hubungi administrator.");

        // Revoke refresh token lama (rotasi token, best practice keamanan)
        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return await BuildResponseAsync(storedToken.User, storedToken.User.Role?.RoleName);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.IsRevoked) return false;

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProfileResponseDto?> GetProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUser == userId && u.DeletedAt == null);

        if (user == null) return null;

        return new ProfileResponseDto
        {
            IdUser = user.IdUser,
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            Image = user.Image,
            RoleName = user.Role?.RoleName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    private async Task<AuthResponseDto> BuildResponseAsync(User user, string? roleName)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        // Generate refresh token baru (random string yang aman)
        var refreshTokenValue = GenerateSecureRandomToken();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        var refreshTokenEntity = new RefreshToken
        {
            IdUser = user.IdUser,
            Token = refreshTokenValue,
            ExpiresAt = refreshTokenExpiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            RefreshToken = refreshTokenValue,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Username = user.Username,
            FullName = user.FullName,
            RoleName = roleName
        };
    }

    private static string GenerateSecureRandomToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
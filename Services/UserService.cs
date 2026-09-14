using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

    public UserService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<PagedResult<UserResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Where(u => u.DeletedAt == null)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => MapToDto(u))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUser == id && u.DeletedAt == null);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
    {
        var roleExists = await _context.Roles.AnyAsync(r => r.IdRole == dto.RoleId);
            if (!roleExists)
                throw new ValidationException($"Role dengan id {dto.RoleId} tidak ditemukan.");

            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);

            if (exists)
                throw new DuplicateDataException($"Email '{dto.Email}' atau Username '{dto.Username}' sudah digunakan.");

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

        await _context.Entry(user).Reference(u => u.Role).LoadAsync();

        return MapToDto(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.IdUser == id && u.DeletedAt == null);

        if (user == null) return null;

        if (dto.Email != null || dto.Username != null)
        {
            var duplicate = await _context.Users.AnyAsync(u =>
                u.IdUser != id &&
                ((dto.Email != null && u.Email == dto.Email) ||
                 (dto.Username != null && u.Username == dto.Username)));

            if (duplicate)
                throw new DuplicateDataException("Email atau Username sudah digunakan.");
        }

        if (dto.RoleId.HasValue)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.IdRole == dto.RoleId.Value);
            if (!roleExists)
                throw new ValidationException($"Role dengan id {dto.RoleId} tidak ditemukan.");

            user.RoleId = dto.RoleId.Value;
        }

        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Username != null) user.Username = dto.Username;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.RoleId.HasValue) user.RoleId = dto.RoleId.Value;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.IdUser == id && u.DeletedAt == null);

        if (user == null) return false;

        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> UpdateImageAsync(int id, IFormFile file)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.IdUser == id && u.DeletedAt == null);

        if (user == null) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            throw new ValidationException("Format gambar tidak didukung. Gunakan jpg, jpeg, png, atau webp.");

        if (file.Length > MaxImageSizeBytes)
            throw new ValidationException("Ukuran gambar maksimal 2MB.");

        var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "users");
        Directory.CreateDirectory(uploadsFolder);

        if (!string.IsNullOrEmpty(user.Image))
        {
            var oldPath = Path.Combine(_env.ContentRootPath, "wwwroot", user.Image.TrimStart('/'));
            if (File.Exists(oldPath)) File.Delete(oldPath);
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/users/{fileName}";
        user.Image = relativePath;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return relativePath;
    }

    private static UserResponseDto MapToDto(User u) => new()
    {
        IdUser = u.IdUser,
        FullName = u.FullName,
        Username = u.Username,
        Email = u.Email,
        IsEmailVerified = u.EmailVerifiedAt != null,
        Image = u.Image,
        RoleId = u.RoleId,
        RoleName = u.Role?.RoleName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class AccessMenuService : IAccessMenuService
{
    private readonly AppDbContext _context;

    public AccessMenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AccessMenuResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.AccessMenus
            .Include(a => a.Role)
            .Include(a => a.Menu)
            .Where(a => a.DeletedAt == null)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<AccessMenuResponseDto?> GetByIdAsync(int id)
    {
        var accessMenu = await _context.AccessMenus
            .Include(a => a.Role)
            .Include(a => a.Menu)
            .FirstOrDefaultAsync(a => a.IdAccessMenu == id && a.DeletedAt == null);

        return accessMenu == null ? null : MapToDto(accessMenu);
    }

    public async Task<AccessMenuResponseDto> CreateAsync(AccessMenuCreateDto dto)
    {
        var roleExists = await _context.Roles.AnyAsync(r => r.IdRole == dto.IdRole);
        if (!roleExists)
            throw new ValidationException($"Role dengan id {dto.IdRole} tidak ditemukan.");

        var menuExists = await _context.Menus.AnyAsync(m => m.IdMenu == dto.IdMenu);
        if (!menuExists)
            throw new ValidationException($"Menu dengan id {dto.IdMenu} tidak ditemukan.");

        var exists = await _context.AccessMenus
            .AnyAsync(a => a.IdRole == dto.IdRole && a.IdMenu == dto.IdMenu && a.DeletedAt == null);

        if (exists)
            throw new DuplicateDataException("Akses menu untuk role dan menu ini sudah ada.");

        var accessMenu = new AccessMenu
        {
            IdRole = dto.IdRole,
            IdMenu = dto.IdMenu,
            CreatedAt = DateTime.UtcNow
        };

        _context.AccessMenus.Add(accessMenu);
        await _context.SaveChangesAsync();

        await _context.Entry(accessMenu).Reference(a => a.Role).LoadAsync();
        await _context.Entry(accessMenu).Reference(a => a.Menu).LoadAsync();

        return MapToDto(accessMenu);
    }

    public async Task<AccessMenuResponseDto?> UpdateAsync(int id, AccessMenuUpdateDto dto)
    {
        var accessMenu = await _context.AccessMenus
            .Include(a => a.Role)
            .Include(a => a.Menu)
            .FirstOrDefaultAsync(a => a.IdAccessMenu == id && a.DeletedAt == null);

        if (accessMenu == null) return null;

        if (dto.IdRole.HasValue)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.IdRole == dto.IdRole.Value);
            if (!roleExists)
                throw new ValidationException($"Role dengan id {dto.IdRole} tidak ditemukan.");
        }

        if (dto.IdMenu.HasValue)
        {
            var menuExists = await _context.Menus.AnyAsync(m => m.IdMenu == dto.IdMenu.Value);
            if (!menuExists)
                throw new ValidationException($"Menu dengan id {dto.IdMenu} tidak ditemukan.");
        }

        if (dto.IdRole.HasValue || dto.IdMenu.HasValue)
        {
            var newIdRole = dto.IdRole ?? accessMenu.IdRole;
            var newIdMenu = dto.IdMenu ?? accessMenu.IdMenu;

            var duplicate = await _context.AccessMenus.AnyAsync(a =>
                a.IdAccessMenu != id &&
                a.IdRole == newIdRole &&
                a.IdMenu == newIdMenu &&
                a.DeletedAt == null);

            if (duplicate)
                throw new DuplicateDataException("Akses menu untuk role dan menu ini sudah ada.");
        }

        if (dto.IdRole.HasValue) accessMenu.IdRole = dto.IdRole.Value;
        if (dto.IdMenu.HasValue) accessMenu.IdMenu = dto.IdMenu.Value;

        accessMenu.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _context.Entry(accessMenu).Reference(a => a.Role).LoadAsync();
        await _context.Entry(accessMenu).Reference(a => a.Menu).LoadAsync();

        return MapToDto(accessMenu);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var accessMenu = await _context.AccessMenus
            .FirstOrDefaultAsync(a => a.IdAccessMenu == id && a.DeletedAt == null);

        if (accessMenu == null) return false;

        accessMenu.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static AccessMenuResponseDto MapToDto(AccessMenu a) => new()
    {
        IdAccessMenu = a.IdAccessMenu,
        IdRole = a.IdRole,
        RoleName = a.Role?.RoleName,
        IdMenu = a.IdMenu,
        MenuName = a.Menu?.MenuName,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
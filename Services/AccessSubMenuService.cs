using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class AccessSubMenuService : IAccessSubMenuService
{
    private readonly AppDbContext _context;

    public AccessSubMenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AccessSubMenuResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.AccessSubMenus
            .Include(a => a.User)
            .Include(a => a.SubMenu)
            .Where(a => a.DeletedAt == null)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<AccessSubMenuResponseDto?> GetByIdAsync(int id)
    {
        var accessSubMenu = await _context.AccessSubMenus
            .Include(a => a.User)
            .Include(a => a.SubMenu)
            .FirstOrDefaultAsync(a => a.IdAccessSubMenu == id && a.DeletedAt == null);

        return accessSubMenu == null ? null : MapToDto(accessSubMenu);
    }

    public async Task<AccessSubMenuResponseDto> CreateAsync(AccessSubMenuCreateDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.IdUser == dto.IdUser);
        if (!userExists)
            throw new ValidationException($"User dengan id {dto.IdUser} tidak ditemukan.");

        var subMenuExists = await _context.SubMenus.AnyAsync(s => s.IdSubMenu == dto.IdSubMenu);
        if (!subMenuExists)
            throw new ValidationException($"SubMenu dengan id {dto.IdSubMenu} tidak ditemukan.");

        var exists = await _context.AccessSubMenus
            .AnyAsync(a => a.IdUser == dto.IdUser && a.IdSubMenu == dto.IdSubMenu && a.DeletedAt == null);

        if (exists)
            throw new DuplicateDataException("Akses submenu untuk user dan submenu ini sudah ada.");

        var accessSubMenu = new AccessSubMenu
        {
            IdUser = dto.IdUser,
            IdSubMenu = dto.IdSubMenu,
            CanView = dto.CanView,
            CanCreate = dto.CanCreate,
            CanUpdate = dto.CanUpdate,
            CanDelete = dto.CanDelete,
            CreatedAt = DateTime.UtcNow
        };

        _context.AccessSubMenus.Add(accessSubMenu);
        await _context.SaveChangesAsync();

        await _context.Entry(accessSubMenu).Reference(a => a.User).LoadAsync();
        await _context.Entry(accessSubMenu).Reference(a => a.SubMenu).LoadAsync();

        return MapToDto(accessSubMenu);
    }

    public async Task<AccessSubMenuResponseDto?> UpdateAsync(int id, AccessSubMenuUpdateDto dto)
    {
        var accessSubMenu = await _context.AccessSubMenus
            .Include(a => a.User)
            .Include(a => a.SubMenu)
            .FirstOrDefaultAsync(a => a.IdAccessSubMenu == id && a.DeletedAt == null);

        if (accessSubMenu == null) return null;

        if (dto.IdUser.HasValue)
        {
            var userExists = await _context.Users.AnyAsync(u => u.IdUser == dto.IdUser.Value);
            if (!userExists)
                throw new ValidationException($"User dengan id {dto.IdUser} tidak ditemukan.");
        }

        if (dto.IdSubMenu.HasValue)
        {
            var subMenuExists = await _context.SubMenus.AnyAsync(s => s.IdSubMenu == dto.IdSubMenu.Value);
            if (!subMenuExists)
                throw new ValidationException($"SubMenu dengan id {dto.IdSubMenu} tidak ditemukan.");
        }

        if (dto.IdUser.HasValue || dto.IdSubMenu.HasValue)
        {
            var newIdUser = dto.IdUser ?? accessSubMenu.IdUser;
            var newIdSubMenu = dto.IdSubMenu ?? accessSubMenu.IdSubMenu;

            var duplicate = await _context.AccessSubMenus.AnyAsync(a =>
                a.IdAccessSubMenu != id &&
                a.IdUser == newIdUser &&
                a.IdSubMenu == newIdSubMenu &&
                a.DeletedAt == null);

            if (duplicate)
                throw new DuplicateDataException("Akses submenu untuk user dan submenu ini sudah ada.");
        }

        if (dto.IdUser.HasValue) accessSubMenu.IdUser = dto.IdUser.Value;
        if (dto.IdSubMenu.HasValue) accessSubMenu.IdSubMenu = dto.IdSubMenu.Value;
        if (dto.CanView.HasValue) accessSubMenu.CanView = dto.CanView.Value;
        if (dto.CanCreate.HasValue) accessSubMenu.CanCreate = dto.CanCreate.Value;
        if (dto.CanUpdate.HasValue) accessSubMenu.CanUpdate = dto.CanUpdate.Value;
        if (dto.CanDelete.HasValue) accessSubMenu.CanDelete = dto.CanDelete.Value;

        accessSubMenu.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _context.Entry(accessSubMenu).Reference(a => a.User).LoadAsync();
        await _context.Entry(accessSubMenu).Reference(a => a.SubMenu).LoadAsync();

        return MapToDto(accessSubMenu);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var accessSubMenu = await _context.AccessSubMenus
            .FirstOrDefaultAsync(a => a.IdAccessSubMenu == id && a.DeletedAt == null);

        if (accessSubMenu == null) return false;

        accessSubMenu.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static AccessSubMenuResponseDto MapToDto(AccessSubMenu a) => new()
    {
        IdAccessSubMenu = a.IdAccessSubMenu,
        IdUser = a.IdUser,
        UserName = a.User?.FullName,
        IdSubMenu = a.IdSubMenu,
        SubMenuName = a.SubMenu?.Title,
        CanView = a.CanView,
        CanCreate = a.CanCreate,
        CanUpdate = a.CanUpdate,
        CanDelete = a.CanDelete,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class SubMenuService : ISubMenuService
{
    private readonly AppDbContext _context;

    public SubMenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SubMenuResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.SubMenus
            .Include(s => s.Menu)
            .Where(s => s.DeletedAt == null)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => MapToDto(s))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<SubMenuResponseDto?> GetByIdAsync(int id)
    {
        var subMenu = await _context.SubMenus
            .Include(s => s.Menu)
            .FirstOrDefaultAsync(s => s.IdSubMenu == id && s.DeletedAt == null);

        return subMenu == null ? null : MapToDto(subMenu);
    }

    public async Task<SubMenuResponseDto> CreateAsync(SubMenuCreateDto dto)
    {
        var menuExists = await _context.Menus.AnyAsync(m => m.IdMenu == dto.IdMenu);
        if (!menuExists)
            throw new ValidationException($"Menu dengan id {dto.IdMenu} tidak ditemukan.");

        if (dto.ParentId.HasValue)
        {
            var parentExists = await _context.SubMenus
                .AnyAsync(s => s.IdSubMenu == dto.ParentId.Value && s.DeletedAt == null);
            if (!parentExists)
                throw new ValidationException($"Parent submenu dengan id {dto.ParentId} tidak ditemukan.");
        }

        var subMenu = new SubMenu
        {
            IdMenu = dto.IdMenu,
            Url = dto.Url,
            Icon = dto.Icon,
            Title = dto.Title,
            Noted = dto.Noted,
            IsActive = dto.IsActive,
            ParentId = dto.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        _context.SubMenus.Add(subMenu);
        await _context.SaveChangesAsync(); // simpan dulu supaya IdSubMenu ke-generate

        // generate kode otomatis berdasarkan Id, contoh: SUB0001
        subMenu.KodeSubMenu = $"SUB{subMenu.IdSubMenu:D4}";
        await _context.SaveChangesAsync();

        await _context.Entry(subMenu).Reference(s => s.Menu).LoadAsync();

        return MapToDto(subMenu);
    }

    public async Task<SubMenuResponseDto?> UpdateAsync(int id, SubMenuUpdateDto dto)
    {
        var subMenu = await _context.SubMenus
            .Include(s => s.Menu)
            .FirstOrDefaultAsync(s => s.IdSubMenu == id && s.DeletedAt == null);

        if (subMenu == null) return null;

        if (dto.IdMenu.HasValue)
        {
            var menuExists = await _context.Menus.AnyAsync(m => m.IdMenu == dto.IdMenu.Value);
            if (!menuExists)
                throw new ValidationException($"Menu dengan id {dto.IdMenu} tidak ditemukan.");

            subMenu.IdMenu = dto.IdMenu.Value;
        }

        if (dto.ParentId.HasValue)
        {
            if (dto.ParentId.Value == id)
                throw new ValidationException("Submenu tidak boleh menjadi parent untuk dirinya sendiri.");

            var parentExists = await _context.SubMenus
                .AnyAsync(s => s.IdSubMenu == dto.ParentId.Value && s.DeletedAt == null);
            if (!parentExists)
                throw new ValidationException($"Parent submenu dengan id {dto.ParentId} tidak ditemukan.");

            subMenu.ParentId = dto.ParentId.Value;
        }

        if (dto.Url != null) subMenu.Url = dto.Url;
        if (dto.Icon != null) subMenu.Icon = dto.Icon;
        if (dto.Title != null) subMenu.Title = dto.Title;
        if (dto.Noted != null) subMenu.Noted = dto.Noted;
        if (dto.IsActive.HasValue) subMenu.IsActive = dto.IsActive.Value;

        subMenu.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(subMenu);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subMenu = await _context.SubMenus
            .FirstOrDefaultAsync(s => s.IdSubMenu == id && s.DeletedAt == null);

        if (subMenu == null) return false;

        subMenu.DeletedAt = DateTime.UtcNow;
        subMenu.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static SubMenuResponseDto MapToDto(SubMenu s) => new()
    {
        IdSubMenu = s.IdSubMenu,
        KodeSubMenu = s.KodeSubMenu,
        IdMenu = s.IdMenu,
        MenuName = s.Menu?.MenuName,
        Url = s.Url,
        Icon = s.Icon,
        Title = s.Title,
        Noted = s.Noted,
        IsActive = s.IsActive,
        ParentId = s.ParentId,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}
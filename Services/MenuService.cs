using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class MenuService : IMenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MenuResponseDto>> GetAllAsync(int page, int pageSize)
{
    return await _context.Menus
        .Where(m => m.DeletedAt == null)
        .OrderBy(m => m.IdMenu)
        .Select(m => new MenuResponseDto
        {
            IdMenu = m.IdMenu,
            KodeMenu = m.KodeMenu,
            MenuName = m.MenuName,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        })
        .ToPagedResultAsync(page, pageSize);
}

    public async Task<MenuResponseDto?> GetByIdAsync(int id)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.IdMenu == id && m.DeletedAt == null);

        if (menu == null) return null;

        return new MenuResponseDto
        {
            IdMenu = menu.IdMenu,
            KodeMenu = menu.KodeMenu,
            MenuName = menu.MenuName,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = menu.UpdatedAt
        };
    }

  public async Task<MenuResponseDto> CreateAsync(MenuCreateDto dto)
{
    var kodeExists = await _context.Menus
        .AnyAsync(m => m.KodeMenu == dto.KodeMenu && m.DeletedAt == null);

    if (kodeExists)
        throw new DuplicateDataException($"Menu dengan kode '{dto.KodeMenu}' sudah ada.");

    var nameExists = await _context.Menus
        .AnyAsync(m => m.MenuName == dto.MenuName && m.DeletedAt == null);

    if (nameExists)
        throw new DuplicateDataException($"Menu dengan nama '{dto.MenuName}' sudah ada.");

    var menu = new Menu
    {
        KodeMenu = dto.KodeMenu,
        MenuName = dto.MenuName,
        CreatedAt = DateTime.UtcNow
    };

    _context.Menus.Add(menu);
    await _context.SaveChangesAsync();

    return new MenuResponseDto
    {
        IdMenu = menu.IdMenu,
        KodeMenu = menu.KodeMenu,
        MenuName = menu.MenuName,
        CreatedAt = menu.CreatedAt,
        UpdatedAt = menu.UpdatedAt
    };
}



public async Task<MenuResponseDto?> UpdateAsync(int id, MenuUpdateDto dto)
{
    var menu = await _context.Menus
        .FirstOrDefaultAsync(m => m.IdMenu == id && m.DeletedAt == null);

    if (menu == null) return null;

    if (dto.KodeMenu != null)
    {
        var duplicateKode = await _context.Menus
            .AnyAsync(m => m.KodeMenu == dto.KodeMenu && m.IdMenu != id && m.DeletedAt == null);

        if (duplicateKode)
            throw new DuplicateDataException($"Menu dengan kode '{dto.KodeMenu}' sudah ada.");

        menu.KodeMenu = dto.KodeMenu;
    }

    if (dto.MenuName != null)
    {
        var duplicateName = await _context.Menus
            .AnyAsync(m => m.MenuName == dto.MenuName && m.IdMenu != id && m.DeletedAt == null);

        if (duplicateName)
            throw new DuplicateDataException($"Menu dengan nama '{dto.MenuName}' sudah ada.");

        menu.MenuName = dto.MenuName;
    }


    menu.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    return new MenuResponseDto
    {
        IdMenu = menu.IdMenu,
        KodeMenu = menu.KodeMenu,
        MenuName = menu.MenuName,
        CreatedAt = menu.CreatedAt,
        UpdatedAt = menu.UpdatedAt
    };
}

        

    public async Task<bool> DeleteAsync(int id)
    {
        var menu = await _context.Menus
            .FirstOrDefaultAsync(m => m.IdMenu == id && m.DeletedAt == null);

        if (menu == null) return false;

        menu.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
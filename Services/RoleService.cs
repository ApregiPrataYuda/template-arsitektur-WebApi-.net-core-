using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RoleResponseDto>> GetAllAsync(int page, int pageSize)
{
    return await _context.Roles
        .Where(r => r.DeletedAt == null)
        .OrderBy(r => r.IdRole)
        .Select(r => new RoleResponseDto
        {
            IdRole = r.IdRole,
            KodeRole = r.KodeRole,
            RoleName = r.RoleName,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        })
        .ToPagedResultAsync(page, pageSize);
}

    public async Task<RoleResponseDto?> GetByIdAsync(int id)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.IdRole == id && r.DeletedAt == null);

        if (role == null) return null;

        return new RoleResponseDto
        {
            IdRole = role.IdRole,
            KodeRole = role.KodeRole,
            RoleName = role.RoleName,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

  public async Task<RoleResponseDto> CreateAsync(RoleCreateDto dto)
{
    var kodeExists = await _context.Roles
        .AnyAsync(r => r.KodeRole == dto.KodeRole && r.DeletedAt == null);

    if (kodeExists)
        throw new DuplicateDataException($"Role dengan kode '{dto.KodeRole}' sudah ada.");

    var nameExists = await _context.Roles
        .AnyAsync(r => r.RoleName == dto.RoleName && r.DeletedAt == null);

    if (nameExists)
        throw new DuplicateDataException($"Role dengan nama '{dto.RoleName}' sudah ada.");

    var role = new Role
    {
        KodeRole = dto.KodeRole,
        RoleName = dto.RoleName,
        Description = dto.Description,
        CreatedAt = DateTime.UtcNow
    };

    _context.Roles.Add(role);
    await _context.SaveChangesAsync();

    return new RoleResponseDto
    {
        IdRole = role.IdRole,
        KodeRole = role.KodeRole,
        RoleName = role.RoleName,
        Description = role.Description,
        CreatedAt = role.CreatedAt,
        UpdatedAt = role.UpdatedAt
    };
}



public async Task<RoleResponseDto?> UpdateAsync(int id, RoleUpdateDto dto)
{
    var role = await _context.Roles
        .FirstOrDefaultAsync(r => r.IdRole == id && r.DeletedAt == null);

    if (role == null) return null;

    if (dto.KodeRole != null)
    {
        var duplicateKode = await _context.Roles
            .AnyAsync(r => r.KodeRole == dto.KodeRole && r.IdRole != id && r.DeletedAt == null);

        if (duplicateKode)
            throw new DuplicateDataException($"Role dengan kode '{dto.KodeRole}' sudah ada.");

        role.KodeRole = dto.KodeRole;
    }

    if (dto.RoleName != null)
    {
        var duplicateName = await _context.Roles
            .AnyAsync(r => r.RoleName == dto.RoleName && r.IdRole != id && r.DeletedAt == null);

        if (duplicateName)
            throw new DuplicateDataException($"Role dengan nama '{dto.RoleName}' sudah ada.");

        role.RoleName = dto.RoleName;
    }

    if (dto.Description != null) role.Description = dto.Description;

    role.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();

    return new RoleResponseDto
    {
        IdRole = role.IdRole,
        KodeRole = role.KodeRole,
        RoleName = role.RoleName,
        Description = role.Description,
        CreatedAt = role.CreatedAt,
        UpdatedAt = role.UpdatedAt
    };
}

        

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.IdRole == id && r.DeletedAt == null);

        if (role == null) return false;

        role.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
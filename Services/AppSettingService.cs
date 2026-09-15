using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class AppSettingService : IAppSettingService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
    private static readonly string[] AllowedFaviconExtensions = { ".ico", ".png", ".svg" };
    private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

    public AppSettingService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<PagedResult<AppSettingResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.AppSettings
            .OrderBy(a => a.Id)
            .Select(a => MapToDto(a))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<AppSettingResponseDto?> GetByIdAsync(int id)
    {
        var appSetting = await _context.AppSettings
            .FirstOrDefaultAsync(a => a.Id == id);

        return appSetting == null ? null : MapToDto(appSetting);
    }

    public async Task<AppSettingResponseDto> CreateAsync(AppSettingCreateDto dto)
    {
        var nameExists = await _context.AppSettings
            .AnyAsync(a => a.AppShortName == dto.AppShortName);

        if (nameExists)
            throw new DuplicateDataException($"App setting dengan short name '{dto.AppShortName}' sudah ada.");

        var appSetting = new AppSetting
        {
            AppName = dto.AppName,
            AppShortName = dto.AppShortName,
            AppTagline = dto.AppTagline,
            AppLogo = dto.AppLogo,
            AppLogoSmall = dto.AppLogoSmall,
            Favicon = dto.Favicon,
            PrimaryColor = dto.PrimaryColor,
            SecondaryColor = dto.SecondaryColor,
            SidebarColor = dto.SidebarColor,
            NavbarColor = dto.NavbarColor,
            FooterText = dto.FooterText,
            FooterLicenseUrl = dto.FooterLicenseUrl,
            FooterDocumentationUrl = dto.FooterDocumentationUrl,
            FooterSupportUrl = dto.FooterSupportUrl,
            Version = dto.Version,
            Environment = dto.Environment,
            CreatedAt = DateTime.UtcNow
        };

        _context.AppSettings.Add(appSetting);
        await _context.SaveChangesAsync();

        return MapToDto(appSetting);
    }

    public async Task<AppSettingResponseDto?> UpdateAsync(int id, AppSettingUpdateDto dto)
    {
        var appSetting = await _context.AppSettings
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appSetting == null) return null;

        if (dto.AppShortName != null)
        {
            var duplicateName = await _context.AppSettings
                .AnyAsync(a => a.AppShortName == dto.AppShortName && a.Id != id);

            if (duplicateName)
                throw new DuplicateDataException($"App setting dengan short name '{dto.AppShortName}' sudah ada.");

            appSetting.AppShortName = dto.AppShortName;
        }

        if (dto.AppName != null) appSetting.AppName = dto.AppName;
        if (dto.AppTagline != null) appSetting.AppTagline = dto.AppTagline;
        if (dto.AppLogo != null) appSetting.AppLogo = dto.AppLogo;
        if (dto.AppLogoSmall != null) appSetting.AppLogoSmall = dto.AppLogoSmall;
        if (dto.Favicon != null) appSetting.Favicon = dto.Favicon;
        if (dto.PrimaryColor != null) appSetting.PrimaryColor = dto.PrimaryColor;
        if (dto.SecondaryColor != null) appSetting.SecondaryColor = dto.SecondaryColor;
        if (dto.SidebarColor != null) appSetting.SidebarColor = dto.SidebarColor;
        if (dto.NavbarColor != null) appSetting.NavbarColor = dto.NavbarColor;
        if (dto.FooterText != null) appSetting.FooterText = dto.FooterText;
        if (dto.FooterLicenseUrl != null) appSetting.FooterLicenseUrl = dto.FooterLicenseUrl;
        if (dto.FooterDocumentationUrl != null) appSetting.FooterDocumentationUrl = dto.FooterDocumentationUrl;
        if (dto.FooterSupportUrl != null) appSetting.FooterSupportUrl = dto.FooterSupportUrl;
        if (dto.Version != null) appSetting.Version = dto.Version;
        if (dto.Environment != null) appSetting.Environment = dto.Environment;

        appSetting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(appSetting);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appSetting = await _context.AppSettings
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appSetting == null) return false;

        _context.AppSettings.Remove(appSetting);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> UpdateLogoAsync(int id, IFormFile file)
        => await UpdateImageFieldAsync(id, file, AllowedImageExtensions, "logo",
            (setting) => setting.AppLogo,
            (setting, path) => setting.AppLogo = path);

    public async Task<string?> UpdateLogoSmallAsync(int id, IFormFile file)
        => await UpdateImageFieldAsync(id, file, AllowedImageExtensions, "logo-small",
            (setting) => setting.AppLogoSmall,
            (setting, path) => setting.AppLogoSmall = path);

    public async Task<string?> UpdateFaviconAsync(int id, IFormFile file)
        => await UpdateImageFieldAsync(id, file, AllowedFaviconExtensions, "favicon",
            (setting) => setting.Favicon,
            (setting, path) => setting.Favicon = path);

    private async Task<string?> UpdateImageFieldAsync(
        int id,
        IFormFile file,
        string[] allowedExtensions,
        string subFolder,
        Func<AppSetting, string?> getCurrentPath,
        Action<AppSetting, string> setNewPath)
    {
        var appSetting = await _context.AppSettings.FirstOrDefaultAsync(a => a.Id == id);
        if (appSetting == null) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new ValidationException($"Format file tidak didukung. Gunakan {string.Join(", ", allowedExtensions)}.");

        if (file.Length > MaxImageSizeBytes)
            throw new ValidationException("Ukuran file maksimal 2MB.");

        var uploadsFolder = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "appsettings", subFolder);
        Directory.CreateDirectory(uploadsFolder);

        var currentPath = getCurrentPath(appSetting);
        if (!string.IsNullOrEmpty(currentPath))
        {
            var oldPath = Path.Combine(_env.ContentRootPath, "wwwroot", currentPath.TrimStart('/'));
            if (File.Exists(oldPath)) File.Delete(oldPath);
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/uploads/appsettings/{subFolder}/{fileName}";
        setNewPath(appSetting, relativePath);
        appSetting.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return relativePath;
    }

    private static AppSettingResponseDto MapToDto(AppSetting a) => new()
    {
        Id = a.Id,
        AppName = a.AppName,
        AppShortName = a.AppShortName,
        AppTagline = a.AppTagline,
        AppLogo = a.AppLogo,
        AppLogoSmall = a.AppLogoSmall,
        Favicon = a.Favicon,
        PrimaryColor = a.PrimaryColor,
        SecondaryColor = a.SecondaryColor,
        SidebarColor = a.SidebarColor,
        NavbarColor = a.NavbarColor,
        FooterText = a.FooterText,
        FooterLicenseUrl = a.FooterLicenseUrl,
        FooterDocumentationUrl = a.FooterDocumentationUrl,
        FooterSupportUrl = a.FooterSupportUrl,
        Version = a.Version,
        Environment = a.Environment,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
using Microsoft.EntityFrameworkCore;
using appOne.Data;
using appOne.DTOs;
using appOne.Models;
using appOne.Helpers;

namespace appOne.Services;

public class AppSettingService : IAppSettingService
{
    private readonly AppDbContext _context;

    public AppSettingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AppSettingResponseDto>> GetAllAsync(int page, int pageSize)
    {
        return await _context.AppSettings
            .OrderBy(a => a.Id)
            .Select(a => new AppSettingResponseDto
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
            })
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<AppSettingResponseDto?> GetByIdAsync(int id)
    {
        var appSetting = await _context.AppSettings
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appSetting == null) return null;

        return new AppSettingResponseDto
        {
            Id = appSetting.Id,
            AppName = appSetting.AppName,
            AppShortName = appSetting.AppShortName,
            AppTagline = appSetting.AppTagline,
            AppLogo = appSetting.AppLogo,
            AppLogoSmall = appSetting.AppLogoSmall,
            Favicon = appSetting.Favicon,
            PrimaryColor = appSetting.PrimaryColor,
            SecondaryColor = appSetting.SecondaryColor,
            SidebarColor = appSetting.SidebarColor,
            NavbarColor = appSetting.NavbarColor,
            FooterText = appSetting.FooterText,
            FooterLicenseUrl = appSetting.FooterLicenseUrl,
            FooterDocumentationUrl = appSetting.FooterDocumentationUrl,
            FooterSupportUrl = appSetting.FooterSupportUrl,
            Version = appSetting.Version,
            Environment = appSetting.Environment,
            CreatedAt = appSetting.CreatedAt,
            UpdatedAt = appSetting.UpdatedAt
        };
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

        return new AppSettingResponseDto
        {
            Id = appSetting.Id,
            AppName = appSetting.AppName,
            AppShortName = appSetting.AppShortName,
            AppTagline = appSetting.AppTagline,
            AppLogo = appSetting.AppLogo,
            AppLogoSmall = appSetting.AppLogoSmall,
            Favicon = appSetting.Favicon,
            PrimaryColor = appSetting.PrimaryColor,
            SecondaryColor = appSetting.SecondaryColor,
            SidebarColor = appSetting.SidebarColor,
            NavbarColor = appSetting.NavbarColor,
            FooterText = appSetting.FooterText,
            FooterLicenseUrl = appSetting.FooterLicenseUrl,
            FooterDocumentationUrl = appSetting.FooterDocumentationUrl,
            FooterSupportUrl = appSetting.FooterSupportUrl,
            Version = appSetting.Version,
            Environment = appSetting.Environment,
            CreatedAt = appSetting.CreatedAt,
            UpdatedAt = appSetting.UpdatedAt
        };
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

        return new AppSettingResponseDto
        {
            Id = appSetting.Id,
            AppName = appSetting.AppName,
            AppShortName = appSetting.AppShortName,
            AppTagline = appSetting.AppTagline,
            AppLogo = appSetting.AppLogo,
            AppLogoSmall = appSetting.AppLogoSmall,
            Favicon = appSetting.Favicon,
            PrimaryColor = appSetting.PrimaryColor,
            SecondaryColor = appSetting.SecondaryColor,
            SidebarColor = appSetting.SidebarColor,
            NavbarColor = appSetting.NavbarColor,
            FooterText = appSetting.FooterText,
            FooterLicenseUrl = appSetting.FooterLicenseUrl,
            FooterDocumentationUrl = appSetting.FooterDocumentationUrl,
            FooterSupportUrl = appSetting.FooterSupportUrl,
            Version = appSetting.Version,
            Environment = appSetting.Environment,
            CreatedAt = appSetting.CreatedAt,
            UpdatedAt = appSetting.UpdatedAt
        };
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
}
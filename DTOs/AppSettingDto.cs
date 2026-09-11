namespace appOne.DTOs;

public class AppSettingCreateDto
{
    public string AppName { get; set; } = string.Empty;
    public string AppShortName { get; set; } = string.Empty;
    public string? AppTagline { get; set; }
    public string? AppLogo { get; set; }
    public string? AppLogoSmall { get; set; }
    public string? Favicon { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SidebarColor { get; set; }
    public string? NavbarColor { get; set; }
    public string? FooterText { get; set; }
    public string? FooterLicenseUrl { get; set; }
    public string? FooterDocumentationUrl { get; set; }
    public string? FooterSupportUrl { get; set; }
    public string? Version { get; set; }
    public string? Environment { get; set; }
}

public class AppSettingUpdateDto
{
    public string? AppName { get; set; }
    public string? AppShortName { get; set; }
    public string? AppTagline { get; set; }
    public string? AppLogo { get; set; }
    public string? AppLogoSmall { get; set; }
    public string? Favicon { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SidebarColor { get; set; }
    public string? NavbarColor { get; set; }
    public string? FooterText { get; set; }
    public string? FooterLicenseUrl { get; set; }
    public string? FooterDocumentationUrl { get; set; }
    public string? FooterSupportUrl { get; set; }
    public string? Version { get; set; }
    public string? Environment { get; set; }
}

public class AppSettingResponseDto
{
    public int Id { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string AppShortName { get; set; } = string.Empty;
    public string? AppTagline { get; set; }
    public string? AppLogo { get; set; }
    public string? AppLogoSmall { get; set; }
    public string? Favicon { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SidebarColor { get; set; }
    public string? NavbarColor { get; set; }
    public string? FooterText { get; set; }
    public string? FooterLicenseUrl { get; set; }
    public string? FooterDocumentationUrl { get; set; }
    public string? FooterSupportUrl { get; set; }
    public string? Version { get; set; }
    public string? Environment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
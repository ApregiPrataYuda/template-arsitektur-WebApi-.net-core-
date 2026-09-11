using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appOne.Models;

[Table("app_settings")]
public class AppSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("app_name")]
    public string AppName { get; set; } = string.Empty;

    [Column("app_short_name")]
    public string AppShortName { get; set; } = string.Empty;

    [Column("app_tagline")]
    public string? AppTagline { get; set; }

    [Column("app_logo")]
    public string? AppLogo { get; set; }

    [Column("app_logo_small")]
    public string? AppLogoSmall { get; set; }

    [Column("favicon")]
    public string? Favicon { get; set; }

    [Column("primary_color")]
    public string? PrimaryColor { get; set; }

    [Column("secondary_color")]
    public string? SecondaryColor { get; set; }

    [Column("sidebar_color")]
    public string? SidebarColor { get; set; }

    [Column("navbar_color")]
    public string? NavbarColor { get; set; }

    [Column("footer_text")]
    public string? FooterText { get; set; }

    [Column("footer_license_url")]
    public string? FooterLicenseUrl { get; set; }

    [Column("footer_documentation_url")]
    public string? FooterDocumentationUrl { get; set; }

    [Column("footer_support_url")]
    public string? FooterSupportUrl { get; set; }

    [Column("version")]
    public string? Version { get; set; }

    [Column("environment")]
    public string? Environment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
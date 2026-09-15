using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appOne.Models;

[Table("ms_submenu")]
public class SubMenu
{
    [Key]
    [Column("id_submenu")]
    public int IdSubMenu { get; set; }

    [Column("kode_submenu")]
    public string KodeSubMenu { get; set; } = string.Empty;

    [Column("id_menu")]
    public int IdMenu { get; set; }

    [Column("url")]
    public string Url { get; set; } = string.Empty;

    [Column("icon")]
    public string? Icon { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("noted")]
    public string? Noted { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("parent_id")]
    public int? ParentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    // Navigation property - relasi ke Menu induk
    public Menu? Menu { get; set; }
}
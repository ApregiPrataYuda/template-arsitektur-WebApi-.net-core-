using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appOne.Models;

[Table("ms_access_submenu")]
public class AccessSubMenu
{
    [Key]
    public int IdAccessSubMenu { get; set; }

    public int IdUser { get; set; }

    public int IdSubMenu { get; set; }

    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanUpdate { get; set; } = false;
    public bool CanDelete { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey(nameof(IdUser))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(IdSubMenu))]
    public SubMenu SubMenu { get; set; } = null!;
}
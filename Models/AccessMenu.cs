using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appOne.Models;

[Table("ms_access_menu")]
public class AccessMenu
{
    [Key]
    public int IdAccessMenu { get; set; }

    public int IdRole { get; set; }

    public int IdMenu { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey(nameof(IdRole))]
    public Role Role { get; set; } = null!;

    [ForeignKey(nameof(IdMenu))]
    public Menu Menu { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appOne.Models;

[Table("ms_menu")]
public class Menu
{
    [Key]
    public int IdMenu { get; set; }

    public string KodeMenu { get; set; } = string.Empty;

    [Column("menu")]
    public string MenuName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
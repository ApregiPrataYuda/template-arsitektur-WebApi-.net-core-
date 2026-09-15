using System.ComponentModel.DataAnnotations;

namespace appOne.DTOs;

public class AccessSubMenuCreateDto
{
    [Required]
    public int IdUser { get; set; }

    [Required]
    public int IdSubMenu { get; set; }

    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanUpdate { get; set; } = false;
    public bool CanDelete { get; set; } = false;
}

public class AccessSubMenuUpdateDto
{
    public int? IdUser { get; set; }
    public int? IdSubMenu { get; set; }

    public bool? CanView { get; set; }
    public bool? CanCreate { get; set; }
    public bool? CanUpdate { get; set; }
    public bool? CanDelete { get; set; }
}

public class AccessSubMenuResponseDto
{
    public int IdAccessSubMenu { get; set; }
    public int IdUser { get; set; }
    public string? UserName { get; set; }
    public int IdSubMenu { get; set; }
    public string? SubMenuName { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
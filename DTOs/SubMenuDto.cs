using System.ComponentModel.DataAnnotations;

namespace appOne.DTOs;

public class SubMenuCreateDto
{
    [Required]
    public int IdMenu { get; set; }

    [Required, MaxLength(255)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Icon { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Noted { get; set; }

    public bool IsActive { get; set; } = true;

    public int? ParentId { get; set; }
}

public class SubMenuUpdateDto
{
    public int? IdMenu { get; set; }

    [MaxLength(255)]
    public string? Url { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(150)]
    public string? Title { get; set; }

    public string? Noted { get; set; }

    public bool? IsActive { get; set; }

    public int? ParentId { get; set; }
}

public class SubMenuResponseDto
{
    public int IdSubMenu { get; set; }
    public string KodeSubMenu { get; set; } = string.Empty;
    public int IdMenu { get; set; }
    public string? MenuName { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Noted { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
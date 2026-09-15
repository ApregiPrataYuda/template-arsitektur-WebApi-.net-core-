using System.ComponentModel.DataAnnotations;

namespace appOne.DTOs;

public class AccessMenuCreateDto
{
    [Required]
    public int IdRole { get; set; }

    [Required]
    public int IdMenu { get; set; }
}

public class AccessMenuUpdateDto
{
    public int? IdRole { get; set; }
    public int? IdMenu { get; set; }
}

public class AccessMenuResponseDto
{
    public int IdAccessMenu { get; set; }
    public int IdRole { get; set; }
    public string? RoleName { get; set; }
    public int IdMenu { get; set; }
    public string? MenuName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
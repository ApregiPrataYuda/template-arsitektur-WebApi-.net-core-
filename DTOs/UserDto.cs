using System.ComponentModel.DataAnnotations;

namespace appOne.DTOs;

public class UserCreateDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }
}

public class UserUpdateDto
{
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }

    [MinLength(8)]
    public string? Password { get; set; }

    public int? RoleId { get; set; }
    public bool? IsActive { get; set; }
}

public class UserResponseDto
{
    public int IdUser { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string? Image { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
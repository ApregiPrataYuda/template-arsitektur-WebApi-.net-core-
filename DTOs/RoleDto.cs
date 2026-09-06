namespace appOne.DTOs;

public class RoleCreateDto
{
    public string KodeRole { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class RoleUpdateDto
{
    public string? KodeRole { get; set; }
    public string? RoleName { get; set; }
    public string? Description { get; set; }
}

public class RoleResponseDto
{
    public int IdRole { get; set; }
    public string KodeRole { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
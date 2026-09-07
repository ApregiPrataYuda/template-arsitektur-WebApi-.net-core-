namespace appOne.DTOs;

public class MenuCreateDto
{
    public string KodeMenu { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
}

public class MenuUpdateDto
{
    public string? KodeMenu { get; set; }
    public string? MenuName { get; set; }
}

public class MenuResponseDto
{
    public int IdMenu { get; set; }
    public string KodeMenu { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
namespace appOne.Helpers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Meta { get; set; }              // ⬅️ tambahan

    public static ApiResponse<T> Ok(T data, string message = "Berhasil", object? meta = null)
        => new() { Success = true, Message = message, Data = data, Meta = meta };  // ⬅️ tambah param meta

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };
}
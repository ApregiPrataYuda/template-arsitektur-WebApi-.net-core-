using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using appOne.DTOs;
using appOne.Services;

namespace appOne.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) { _service = service; }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
        => Ok(await _service.RegisterAsync(dto));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _service.LoginAsync(dto);
        if (result == null) return Unauthorized(new { error = "Username/email atau password salah." });
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var result = await _service.RefreshTokenAsync(dto.RefreshToken);
        if (result == null) return Unauthorized(new { error = "Refresh token tidak valid atau sudah kadaluarsa." });
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
    {
        var success = await _service.RevokeRefreshTokenAsync(dto.RefreshToken);
        return success
            ? Ok(new { message = "Logout berhasil." })
            : BadRequest(new { error = "Refresh token tidak ditemukan." });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var profile = await _service.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        return Ok(profile);
    }
}
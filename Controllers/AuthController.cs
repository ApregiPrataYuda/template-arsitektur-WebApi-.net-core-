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

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        // Ambil id user dari klaim di dalam token JWT yang sudah divalidasi middleware
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var profile = await _service.GetProfileAsync(userId);
        if (profile == null) return NotFound();

        return Ok(profile);
    }
}
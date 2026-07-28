using CoffeeKiosk.Application.Common;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeKiosk.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto , CancellationToken ct)
    {
        var log = await _authService.LoginAsync(loginDto, ct);
        return Ok(log);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("users")]
    public async Task<IActionResult> CreateKiosk(CreateUserDto dto, CancellationToken ct)
    {
        var create = await _authService.CreateUserAsync(dto, ct);
        return Ok(create);
    }
    
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("users/list")]
    public async Task<IActionResult> GetAllPersonal(CancellationToken ct)
    {
        var get = await _authService.GetAllUsersAsync(ct);
        return Ok(get);
    }
}
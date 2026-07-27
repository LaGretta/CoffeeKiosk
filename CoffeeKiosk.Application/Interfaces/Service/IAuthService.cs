using CoffeeKiosk.Application.DTOs;

namespace CoffeeKiosk.Application.Interfaces.Service;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<AuthResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct);
    Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct);
}
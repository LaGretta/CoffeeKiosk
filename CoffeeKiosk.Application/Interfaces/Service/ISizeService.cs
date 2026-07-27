using CoffeeKiosk.Application.DTOs;

namespace CoffeeKiosk.Application.Interfaces.Service;

public interface ISizeService
{
    Task<List<SizeResponseDto>> GetAllAsync(CancellationToken ct);
    Task<SizeResponseDto> GetByIdAsync(int id, CancellationToken ct);
    Task<SizeResponseDto> CreateAsync(CreateSizeDto dto, CancellationToken ct);
    Task<SizeResponseDto> UpdateAsync(int id, CreateSizeDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
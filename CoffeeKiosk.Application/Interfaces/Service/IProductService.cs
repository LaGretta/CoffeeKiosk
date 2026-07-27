using CoffeeKiosk.Application.DTOs;

namespace CoffeeKiosk.Application.Interfaces.Service;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(CancellationToken ct);
    Task<ProductResponseDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken ct);
    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<List<ProductResponseDto>> GetMenuAsync(CancellationToken ct);
}
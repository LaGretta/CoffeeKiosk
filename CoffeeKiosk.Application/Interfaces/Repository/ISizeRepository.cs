using CoffeeKiosk.Domain.Entities;
namespace CoffeeKiosk.Application.Interfaces.Repository;

public interface ISizeRepository
{
    Task<List<Size>> GetAllAsync(CancellationToken ct);
    Task<Size?> GetByIdAsync(int id, CancellationToken ct);
    
    Task<Size> AddAsync(Size size, CancellationToken ct);
    void Update(Size size);
    void Delete(Size size);
}
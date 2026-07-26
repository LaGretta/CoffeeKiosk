using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Interfaces.Repository;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct);
    Task<List<Order>> GetQueueAsync(CancellationToken ct);
    Task<int> GetTodayOrderCountAsync(CancellationToken ct);
    void Update(Order order);
}
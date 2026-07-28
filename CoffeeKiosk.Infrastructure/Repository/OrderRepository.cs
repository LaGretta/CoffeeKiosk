using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Domain.Enums;
using CoffeeKiosk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await _context.Orders.AddAsync(order, ct);
    }
    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Size)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }
    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Size)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);
    }
    public async Task<List<Order>> GetQueueAsync(CancellationToken ct)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Size)
            .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(ct);
    }
    public async Task<int> GetTodayOrderCountAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Orders
            .CountAsync(o => o.CreatedAt >= today, ct);
    }
    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }
}
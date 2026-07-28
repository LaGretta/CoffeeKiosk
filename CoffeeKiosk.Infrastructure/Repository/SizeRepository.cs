using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Repository;

public class SizeRepository : ISizeRepository
{
    private readonly AppDbContext _context;
    public SizeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Size>> GetAllAsync(CancellationToken ct)
    {
        var get = await _context.Sizes.ToListAsync(ct);
        return get;
    }
    public async Task<Size?> GetByIdAsync(int id, CancellationToken ct)
    {
        var get = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id, ct);
        return get;
    }
    public async Task AddAsync(Size size, CancellationToken ct)
    {
        await _context.Sizes.AddAsync(size, ct);
    }

    public void Update(Size size)
    {
        _context.Update(size);   
    }

    public void Delete(Size size)
    {
        _context.Remove(size);
    }
}
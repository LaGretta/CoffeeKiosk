using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        var getuser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        return getuser;
    }
    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        var getbyId = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        return getbyId;
    }
    public async Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        var getall =  await _context.Users.ToListAsync(ct);
        return getall;
    }
    public async Task AddAsync(User user, CancellationToken ct)
    {
         await _context.Users.AddAsync(user, ct);
    }
}
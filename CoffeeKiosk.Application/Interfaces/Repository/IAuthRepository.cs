using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Interfaces.Repository;

public interface IAuthRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
using CoffeeKiosk.Application.Interfaces.Security;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher hasher, CancellationToken ct = default)
    {
        if (await context.Users.AnyAsync(u => u.Role == Role.Admin, ct))
            return;

        var admin = new User
        {
            Username = "admin",
            Email = "admin@coffeekiosk.local",
            PasswordHash = hasher.Hash("admin123"),
            Role = Role.Admin,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync(ct);
    }
}
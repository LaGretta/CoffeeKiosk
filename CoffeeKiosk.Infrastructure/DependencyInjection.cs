using CoffeeKiosk.Application.Interfaces;
using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Application.Interfaces.Security;
using CoffeeKiosk.Infrastructure.Data;
using CoffeeKiosk.Infrastructure.Repository;
using CoffeeKiosk.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeKiosk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISizeRepository, SizeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
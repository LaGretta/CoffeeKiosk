using System.Reflection;
using CoffeeKiosk.Application.Interfaces.Service;
using CoffeeKiosk.Application.Service;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeKiosk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg
            .AddMaps(Assembly.GetExecutingAssembly()));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISizeService, SizeService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
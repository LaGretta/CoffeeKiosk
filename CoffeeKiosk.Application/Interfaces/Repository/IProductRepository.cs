using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Interfaces.Repository;

public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync(CancellationToken ct);
    Task<Product?> GetProductAsync(int id, CancellationToken ct);
    
    Task CreateProductAsync(Product product, CancellationToken ct);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
}
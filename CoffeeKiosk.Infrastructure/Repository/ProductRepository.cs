using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeKiosk.Infrastructure.Repository;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct)
    {
        return await _context.Products.ToListAsync(ct);
    }
    public async Task<Product?> GetProductAsync(int id, CancellationToken ct)
    {
         var getProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
         return getProduct;
    }

    public async Task CreateProductAsync(Product product, CancellationToken ct)
    {
         await  _context.Products.AddAsync(product, ct);
    }

    public void UpdateProduct(Product product)
    {
        _context.Products.Update(product);
    }

    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
    }
}
using CoffeeKiosk.Application.Common;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeKiosk.API.Controllers;
[Authorize (Roles = Roles.Admin)]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken ct)
    {
        var get = await _productService.GetAllAsync(ct);
        return Ok(get);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductsById(int id, CancellationToken ct)
    {
        var get = await _productService.GetByIdAsync(id, ct);
        return Ok(get);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto,CancellationToken ct)
    {
        var created = await _productService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetProductsById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id ,UpdateProductDto dto, CancellationToken ct)
    { 
        var updated = await _productService.UpdateAsync(id, dto, ct);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id ,CancellationToken ct)
    {
       await _productService.DeleteAsync(id, ct);
       return NoContent();
    }
}
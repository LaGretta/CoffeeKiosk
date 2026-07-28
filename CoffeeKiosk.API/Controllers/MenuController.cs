using CoffeeKiosk.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeKiosk.API.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController  : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ISizeService _sizeService;
    public MenuController(IProductService productService, ISizeService sizeService)
    {
        _productService = productService;
        _sizeService = sizeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMenuAsync(CancellationToken ct)
    {
        var menu = await _productService.GetMenuAsync(ct);
        var size = await _sizeService.GetAllAsync(ct);
        
        return Ok(new { menu, size });
    }
}
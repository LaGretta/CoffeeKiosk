using CoffeeKiosk.Domain.Enums;

namespace CoffeeKiosk.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public Category Category { get; set; }
    public bool IsAvailable { get; set; }
}
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public Category Category { get; set; }
    public bool IsAvailable { get; set; }
}
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public Category Category { get; set; }
    public bool IsAvailable { get; set; }
}
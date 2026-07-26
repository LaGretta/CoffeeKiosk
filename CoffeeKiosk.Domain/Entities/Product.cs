using CoffeeKiosk.Domain.Enums;

namespace CoffeeKiosk.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public Category Category { get; set; }
    public bool IsAvailable { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
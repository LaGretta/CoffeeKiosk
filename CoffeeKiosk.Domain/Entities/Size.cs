namespace CoffeeKiosk.Domain.Entities;

public class Size
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;  
    public decimal PriceModifier { get; set; }        
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
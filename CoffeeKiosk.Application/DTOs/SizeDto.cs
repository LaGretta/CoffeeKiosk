namespace CoffeeKiosk.Application.DTOs;

public class CreateSizeDto
{
    public string Name { get; set; } = string.Empty;  
    public decimal PriceModifier { get; set; }        
}
public class SizeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
}
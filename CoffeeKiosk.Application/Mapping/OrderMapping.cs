using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, OrderResponseDto>();

        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.SizeName, o => o.MapFrom(s => s.Size.Name));
    }
}
using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Domain.Entities;


namespace CoffeeKiosk.Application.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<Product, ProductResponseDto>();

        CreateMap<CreateSizeDto, Size>();
        CreateMap<Size, SizeResponseDto>();
    }
}
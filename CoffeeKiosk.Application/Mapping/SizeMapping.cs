using CoffeeKiosk.Domain.Entities;
using AutoMapper;
using CoffeeKiosk.Application.DTOs;

namespace CoffeeKiosk.Application.Mapping;

public class SizeMapping : Profile
{
    public SizeMapping()
    {
        CreateMap<CreateSizeDto, Size>();
        CreateMap<Size, SizeResponseDto>();
    }
}
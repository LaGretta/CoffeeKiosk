using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Mapping;

public class AuthMapping : Profile
{
    public AuthMapping()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<LoginDto, User>();
        CreateMap<User,AuthResponseDto>();
        CreateMap<User, UserResponseDto>();
    }
}
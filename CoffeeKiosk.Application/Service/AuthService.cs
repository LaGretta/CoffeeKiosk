using System.Text;
using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces;
using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Application.Interfaces.Security;
using CoffeeKiosk.Application.Interfaces.Service;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Domain.Enums;

namespace CoffeeKiosk.Application.Service;

public class AuthService : IAuthService 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _hasher;
    
    public AuthService(IUnitOfWork unitOfWork
        , IAuthRepository authRepository
        , IMapper mapper
        , IJwtTokenGenerator jwtTokenGenerator
        , IPasswordHasher hasher)
    {
        _unitOfWork = unitOfWork;
        _authRepository = authRepository;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _hasher = hasher;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
    {
         var user = await _authRepository.GetByUsernameAsync(dto.Username, ct);
         if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
             throw new KeyNotFoundException("User or Password does not exist");
         
         var response = _mapper.Map<AuthResponseDto>(user);
         response.Token = _jwtTokenGenerator.GenerateJwtToken(user);
         return response;
    }

    public async Task<AuthResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct)
    {
        var existing = await _authRepository.GetByUsernameAsync(dto.Username, ct);
        if (existing != null)
            throw new UnauthorizedAccessException("Invalid username or password");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _authRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = _jwtTokenGenerator.GenerateJwtToken(user)
        };
      
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync(CancellationToken ct)
    {
        var find = await _authRepository.GetAllAsync(ct);
        return _mapper.Map<List<UserResponseDto>>(find);
    }
}
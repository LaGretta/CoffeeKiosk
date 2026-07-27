using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}
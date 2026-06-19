using SmartMetering.Domain.Entities;

namespace SmartMetering.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}

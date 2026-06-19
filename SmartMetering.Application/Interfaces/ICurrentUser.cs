using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    UserRole Role { get; }
}

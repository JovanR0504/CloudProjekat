using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    public string Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Email)
                ?? string.Empty;
        }
    }

    public UserRole Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Role);

            return Enum.TryParse<UserRole>(role, out var parsed) ? parsed : UserRole.Consumer;
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Auth;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is null)
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));

        if (!user.IsActive)
            return Result<LoginResponse>.Failure(
                Error.Unauthorized("Auth.InactiveAccount", "Account is not active."));

        var token = _jwtService.GenerateToken(user);

        var response = new LoginResponse(
            token,
            DateTime.UtcNow.AddMinutes(60),
            user.Role.ToString());

        return Result<LoginResponse>.Success(response);
    }
}

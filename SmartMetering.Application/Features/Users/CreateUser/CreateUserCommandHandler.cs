using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Users;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(IAppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == emailNormalized, cancellationToken);

        if (existingUser)
            return Result<UserResponse>.Failure(
                Error.Conflict("Users.EmailTaken", "A user with this email already exists."));

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return Result<UserResponse>.Failure(
                Error.Validation("Users.InvalidRole", "Invalid role specified."));

        var userResult = User.Create(
            request.Email,
            string.Empty,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            role);

        if (userResult.IsFailure)
            return Result<UserResponse>.Failure(userResult.Error!);

        var user = userResult.Value;
        _context.Users.Add(user);

        var rawToken = Guid.NewGuid().ToString("N");
        var tokenHash = Convert.ToHexString(SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(rawToken))).ToLowerInvariant();

        var tokenResult = PasswordToken.Create(
            user.Id,
            tokenHash,
            TokenPurpose.AccountActivation,
            DateTime.UtcNow.AddHours(48));

        if (tokenResult.IsFailure)
            return Result<UserResponse>.Failure(tokenResult.Error!);

        _context.PasswordTokens.Add(tokenResult.Value);

        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendActivationEmailAsync(user.Email, user.FirstName, rawToken);

        var response = new UserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Role.ToString(),
            user.IsActive,
            user.CreatedAtUtc);

        return Result<UserResponse>.Success(response);
    }
}

using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.SetPassword;

public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, Result>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordService _passwordService;

    public SetPasswordCommandHandler(IAppDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<Result> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(request.Token))).ToLowerInvariant();

        var passwordToken = await _context.PasswordTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (passwordToken is null)
            return Result.Failure(Error.Validation("Auth.InvalidToken", "Invalid or expired token."));

        if (passwordToken.IsExpired())
            return Result.Failure(Error.Validation("Auth.TokenExpired", "Token has expired."));

        if (passwordToken.IsUsed())
            return Result.Failure(Error.Validation("Auth.TokenUsed", "Token has already been used."));

        if (request.Password != request.ConfirmPassword)
            return Result.Failure(Error.Validation("Auth.PasswordMismatch", "Passwords do not match."));

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == passwordToken.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound("Auth.UserNotFound", "User not found."));

        var hashedPassword = _passwordService.Hash(request.Password);
        user.SetPasswordHash(hashedPassword);
        user.Activate();

        passwordToken.MarkUsed();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

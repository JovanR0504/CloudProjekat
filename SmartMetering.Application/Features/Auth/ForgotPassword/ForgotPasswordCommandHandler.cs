using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAppDbContext _context;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IAppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is not null)
        {
            var rawToken = Guid.NewGuid().ToString("N");
            var tokenHash = Convert.ToHexString(SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(rawToken))).ToLowerInvariant();

            var tokenResult = PasswordToken.Create(
                user.Id,
                tokenHash,
                TokenPurpose.PasswordReset,
                DateTime.UtcNow.AddHours(24));

            if (tokenResult.IsSuccess)
            {
                _context.PasswordTokens.Add(tokenResult.Value);
                await _context.SaveChangesAsync(cancellationToken);

                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, rawToken);
            }
        }

        // Always return success to not leak user existence
        return Result.Success();
    }
}

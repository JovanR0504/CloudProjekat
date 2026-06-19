using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Meters.RegisterDevice;

public class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, Result<RegisterDeviceResponse>>
{
    private readonly IAppDbContext _context;

    public RegisterDeviceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RegisterDeviceResponse>> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        var meter = await _context.Meters
            .FirstOrDefaultAsync(m => m.SerialNumber == request.SerialNumber.Trim().ToUpperInvariant(), cancellationToken);

        if (meter is null)
            return Result<RegisterDeviceResponse>.Failure(
                Error.NotFound("Meters.NotFound", "Meter with this serial number not found."));

        if (meter.Status != MeterStatus.Registered)
            return Result<RegisterDeviceResponse>.Failure(
                Error.Validation("Meters.InvalidStatus", "Meter is not in a valid state for device registration."));

        var rawToken = GenerateAccessToken();
        var tokenHash = HashToken(rawToken);

        var activateResult = meter.Activate(request.DeviceId, tokenHash);
        if (activateResult.IsFailure)
            return Result<RegisterDeviceResponse>.Failure(activateResult.Error!);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<RegisterDeviceResponse>.Success(new RegisterDeviceResponse(rawToken));
    }

    private static string GenerateAccessToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

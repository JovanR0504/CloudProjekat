using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;
using SmartMetering.Domain.Helpers;

namespace SmartMetering.Application.Features.Meters.CreateMeter;

public class CreateMeterCommandHandler : IRequestHandler<CreateMeterCommand, Result<MeterResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateMeterCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MeterResponse>> Handle(CreateMeterCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result<MeterResponse>.Failure(
                Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result<MeterResponse>.Failure(
                Error.Unauthorized("Properties.Forbidden", "You do not own this property."));

        if (!Enum.TryParse<ConnectionType>(request.ConnectionType, ignoreCase: true, out var connectionType))
            return Result<MeterResponse>.Failure(
                Error.Validation("Meters.InvalidConnectionType", "Connection type must be 'SinglePhase' or 'ThreePhase'."));

        if (!SerialNumberValidator.IsValid(request.SerialNumber))
            return Result<MeterResponse>.Failure(
                Error.Validation("Meters.InvalidSerialNumber", "Serial number must match format SM-YYYY-XXXXX."));

        var serialExists = await _context.Meters
            .AnyAsync(m => m.SerialNumber == request.SerialNumber.Trim().ToUpperInvariant(), cancellationToken);

        if (serialExists)
            return Result<MeterResponse>.Failure(
                Error.Conflict("Meters.DuplicateSerialNumber", "A meter with this serial number already exists."));

        var meterResult = Meter.Create(
            request.PropertyId,
            request.Label,
            request.SerialNumber,
            connectionType,
            request.Notes);

        if (meterResult.IsFailure)
            return Result<MeterResponse>.Failure(meterResult.Error!);

        var meter = meterResult.Value;
        _context.Meters.Add(meter);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new MeterResponse(
            meter.Id,
            meter.PropertyId,
            meter.Label,
            meter.SerialNumber,
            meter.ConnectionType.ToString(),
            meter.ApprovedPowerKw,
            meter.Status.ToString(),
            meter.Notes,
            meter.DeviceId,
            meter.CreatedAtUtc);

        return Result<MeterResponse>.Success(response);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.GetMeterById;

public class GetMeterByIdQueryHandler : IRequestHandler<GetMeterByIdQuery, Result<MeterResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMeterByIdQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MeterResponse>> Handle(GetMeterByIdQuery request, CancellationToken cancellationToken)
    {
        var meter = await _context.Meters
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.MeterId, cancellationToken);

        if (meter is null)
            return Result<MeterResponse>.Failure(
                Error.NotFound("Meters.NotFound", "Meter not found."));

        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == meter.PropertyId, cancellationToken);

        if (property is null)
            return Result<MeterResponse>.Failure(
                Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result<MeterResponse>.Failure(
                Error.Unauthorized("Meters.Forbidden", "You do not own this meter."));

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

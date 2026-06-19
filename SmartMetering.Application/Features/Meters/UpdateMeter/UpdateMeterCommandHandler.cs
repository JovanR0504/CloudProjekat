using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.UpdateMeter;

public class UpdateMeterCommandHandler : IRequestHandler<UpdateMeterCommand, Result>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateMeterCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateMeterCommand request, CancellationToken cancellationToken)
    {
        var meter = await _context.Meters
            .FirstOrDefaultAsync(m => m.Id == request.MeterId, cancellationToken);

        if (meter is null)
            return Result.Failure(Error.NotFound("Meters.NotFound", "Meter not found."));

        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == meter.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure(Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result.Failure(Error.Unauthorized("Meters.Forbidden", "You do not own this meter."));

        meter.UpdateInfo(request.Label, request.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

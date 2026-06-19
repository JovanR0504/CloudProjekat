using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.GetPropertyById;

public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, Result<PropertyDetailResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetPropertyByIdQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PropertyDetailResponse>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result<PropertyDetailResponse>.Failure(
                Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result<PropertyDetailResponse>.Failure(
                Error.Unauthorized("Properties.Forbidden", "You do not own this property."));

        var meters = await _context.Meters
            .AsNoTracking()
            .Where(m => m.PropertyId == request.PropertyId)
            .OrderBy(m => m.CreatedAtUtc)
            .Select(m => new MeterSummary(
                m.Id,
                m.Label,
                m.SerialNumber,
                m.ConnectionType.ToString(),
                m.ApprovedPowerKw,
                m.Status.ToString()))
            .ToListAsync(cancellationToken);

        var response = new PropertyDetailResponse(
            property.Id,
            property.Name,
            property.City,
            property.Address,
            property.Description,
            property.UserId,
            property.CreatedAtUtc,
            meters);

        return Result<PropertyDetailResponse>.Success(response);
    }
}

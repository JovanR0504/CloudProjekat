using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.GetProperties;

public class GetPropertiesQueryHandler : IRequestHandler<GetPropertiesQuery, Result<List<PropertyResponse>>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetPropertiesQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<PropertyResponse>>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await _context.Properties
            .AsNoTracking()
            .Where(p => p.UserId == _currentUser.UserId)
            .OrderBy(p => p.CreatedAtUtc)
            .Select(p => new PropertyResponse(
                p.Id,
                p.Name,
                p.City,
                p.Address,
                p.Description,
                p.UserId,
                p.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result<List<PropertyResponse>>.Success(properties);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Alerts;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Alerts.GetAlerts;

public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, Result<List<AlertResponse>>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAlertsQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AlertResponse>>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Alerts.AsNoTracking();

        if (_currentUser.Role != UserRole.Admin)
        {
            query = query.Where(a => a.UserId == _currentUser.UserId);
        }

        var alerts = await query
            .OrderByDescending(a => a.OccurredAtUtc)
            .Select(a => new AlertResponse(
                a.Id,
                a.AlertType.ToString(),
                a.Severity.ToString(),
                a.MeterId,
                a.PropertyId,
                a.UserId,
                a.Description,
                a.OccurredAtUtc,
                a.ResolvedAtUtc))
            .ToListAsync(cancellationToken);

        return Result<List<AlertResponse>>.Success(alerts);
    }
}

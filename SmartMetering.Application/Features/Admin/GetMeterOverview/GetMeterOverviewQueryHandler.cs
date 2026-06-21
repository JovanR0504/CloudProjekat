using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Admin;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Admin.GetMeterOverview;

public class GetMeterOverviewQueryHandler : IRequestHandler<GetMeterOverviewQuery, Result<List<MeterOverviewResponse>>>
{
    private readonly IAppDbContext _context;

    public GetMeterOverviewQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MeterOverviewResponse>>> Handle(GetMeterOverviewQuery request, CancellationToken cancellationToken)
    {
        var meters = await (
            from m in _context.Meters.AsNoTracking()
            join p in _context.Properties.AsNoTracking() on m.PropertyId equals p.Id
            join u in _context.Users.AsNoTracking() on p.UserId equals u.Id
            orderby m.CreatedAtUtc descending
            select new MeterOverviewResponse(
                m.Id,
                m.Label,
                m.SerialNumber,
                m.Status.ToString(),
                m.ConnectionType.ToString(),
                m.ApprovedPowerKw,
                m.DeviceId,
                p.Name,
                p.City,
                p.Address,
                u.FirstName + " " + u.LastName,
                u.Email))
            .ToListAsync(cancellationToken);

        return Result<List<MeterOverviewResponse>>.Success(meters);
    }
}

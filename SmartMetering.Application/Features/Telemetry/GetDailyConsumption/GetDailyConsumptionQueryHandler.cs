using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.Telemetry.GetDailyConsumption;

public class GetDailyConsumptionQueryHandler
    : IRequestHandler<GetDailyConsumptionQuery, Result<DailyConsumptionResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ITelemetryStore _telemetryStore;

    public GetDailyConsumptionQueryHandler(
        IAppDbContext context,
        ICurrentUser currentUser,
        ITelemetryStore telemetryStore)
    {
        _context = context;
        _currentUser = currentUser;
        _telemetryStore = telemetryStore;
    }

    public async Task<Result<DailyConsumptionResponse>> Handle(
        GetDailyConsumptionQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result<DailyConsumptionResponse>.Failure(
                Error.NotFound("Telemetry.PropertyNotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result<DailyConsumptionResponse>.Failure(
                Error.Unauthorized("Telemetry.Forbidden", "You do not own this property."));

        var meterIds = await _context.Meters
            .AsNoTracking()
            .Where(m => m.PropertyId == request.PropertyId)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var endDate = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = endDate.AddDays(-6);

        var from = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var to = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var allReadings = new List<Domain.Entities.Telemetry>();

        foreach (var meterId in meterIds)
        {
            var readings = await _telemetryStore.QueryAsync(meterId, from, to);
            allReadings.AddRange(readings);
        }

        var points = allReadings
            .GroupBy(r => DateOnly.FromDateTime(r.RecordedAtUtc))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var ordered = g.OrderBy(r => r.RecordedAtUtc).ToList();

                double vtKwh = 0;
                double ntKwh = 0;

                for (int i = 1; i < ordered.Count; i++)
                {
                    var delta = ordered[i].TotalKwh - ordered[i - 1].TotalKwh;
                    if (delta < 0) continue;

                    if (ordered[i].TariffType == TariffType.HighTariff)
                        vtKwh += delta;
                    else
                        ntKwh += delta;
                }

                return new DailyConsumptionPoint(
                    g.Key.ToString("yyyy-MM-dd"),
                    Math.Round(vtKwh, 3),
                    Math.Round(ntKwh, 3));
            })
            .ToList();

        var response = new DailyConsumptionResponse(
            request.PropertyId,
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"),
            points);

        return Result<DailyConsumptionResponse>.Success(response);
    }
}

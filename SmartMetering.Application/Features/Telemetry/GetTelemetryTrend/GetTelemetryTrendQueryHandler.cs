using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Telemetry.GetTelemetryTrend;

public class GetTelemetryTrendQueryHandler
    : IRequestHandler<GetTelemetryTrendQuery, Result<TelemetryTrendResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ITelemetryStore _telemetryStore;

    public GetTelemetryTrendQueryHandler(
        IAppDbContext context,
        ICurrentUser currentUser,
        ITelemetryStore telemetryStore)
    {
        _context = context;
        _currentUser = currentUser;
        _telemetryStore = telemetryStore;
    }

    public async Task<Result<TelemetryTrendResponse>> Handle(
        GetTelemetryTrendQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result<TelemetryTrendResponse>.Failure(
                Error.NotFound("Telemetry.PropertyNotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result<TelemetryTrendResponse>.Failure(
                Error.Unauthorized("Telemetry.Forbidden", "You do not own this property."));

        var to = request.ToUtc ?? DateTime.UtcNow;
        var from = request.FromUtc ?? to.AddHours(-1);

        var meterIds = await _context.Meters
            .AsNoTracking()
            .Where(m => m.PropertyId == request.PropertyId)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var samples = new List<TelemetrySample>();

        foreach (var meterId in meterIds)
        {
            var readings = await _telemetryStore.QueryAsync(meterId, from, to);

            samples.AddRange(readings.Select(r => new TelemetrySample(
                r.MeterId,
                r.RecordedAtUtc,
                r.Voltage,
                r.LoadKw)));
        }

        var orderedSamples = samples
            .OrderBy(s => s.RecordedAtUtc)
            .ToList();

        var response = new TelemetryTrendResponse(
            request.PropertyId,
            from,
            to,
            orderedSamples);

        return Result<TelemetryTrendResponse>.Success(response);
    }
}

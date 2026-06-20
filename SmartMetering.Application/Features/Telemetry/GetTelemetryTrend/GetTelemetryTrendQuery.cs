using MediatR;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Telemetry.GetTelemetryTrend;

public record GetTelemetryTrendQuery(Guid PropertyId, DateTime? FromUtc, DateTime? ToUtc)
    : IRequest<Result<TelemetryTrendResponse>>;

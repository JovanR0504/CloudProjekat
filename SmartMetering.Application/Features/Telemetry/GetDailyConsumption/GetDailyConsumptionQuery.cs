using MediatR;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Telemetry.GetDailyConsumption;

public record GetDailyConsumptionQuery(Guid PropertyId, DateOnly? Date) : IRequest<Result<DailyConsumptionResponse>>;

using MediatR;
using SmartMetering.Contracts.ConsumptionLimits;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.ConsumptionLimits.GetLimit;

public record GetConsumptionLimitQuery : IRequest<Result<ConsumptionLimitResponse?>>;

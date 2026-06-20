using MediatR;
using SmartMetering.Contracts.ConsumptionLimits;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.ConsumptionLimits.SetLimit;

public record SetConsumptionLimitCommand(decimal Threshold, string Unit) : IRequest<Result<ConsumptionLimitResponse>>;

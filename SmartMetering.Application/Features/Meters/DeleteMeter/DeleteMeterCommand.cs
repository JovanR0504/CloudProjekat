using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.DeleteMeter;

public record DeleteMeterCommand(Guid MeterId) : IRequest<Result>;

using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.UpdateMeter;

public record UpdateMeterCommand(
    Guid MeterId,
    string Label,
    string? Notes) : IRequest<Result>;

using MediatR;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.CreateMeter;

public record CreateMeterCommand(
    Guid PropertyId,
    string Label,
    string SerialNumber,
    string ConnectionType,
    string? Notes) : IRequest<Result<MeterResponse>>;

using MediatR;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.RegisterDevice;

public record RegisterDeviceCommand(
    string DeviceId,
    string SerialNumber) : IRequest<Result<RegisterDeviceResponse>>;

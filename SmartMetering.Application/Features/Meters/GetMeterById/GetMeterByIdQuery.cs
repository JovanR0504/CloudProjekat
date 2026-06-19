using MediatR;
using SmartMetering.Contracts.Meters;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Meters.GetMeterById;

public record GetMeterByIdQuery(Guid MeterId) : IRequest<Result<MeterResponse>>;

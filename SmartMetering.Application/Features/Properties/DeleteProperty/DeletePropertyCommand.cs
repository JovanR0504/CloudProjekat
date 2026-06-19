using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.DeleteProperty;

public record DeletePropertyCommand(Guid PropertyId) : IRequest<Result>;

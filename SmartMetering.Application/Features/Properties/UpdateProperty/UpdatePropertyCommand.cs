using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.UpdateProperty;

public record UpdatePropertyCommand(
    Guid PropertyId,
    string Name,
    string City,
    string Address,
    string? Description) : IRequest<Result>;

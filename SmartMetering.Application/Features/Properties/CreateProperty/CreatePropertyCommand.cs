using MediatR;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.CreateProperty;

public record CreatePropertyCommand(
    string Name,
    string City,
    string Address,
    string? Description) : IRequest<Result<PropertyResponse>>;

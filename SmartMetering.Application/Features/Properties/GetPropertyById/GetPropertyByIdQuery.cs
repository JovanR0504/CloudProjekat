using MediatR;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.GetPropertyById;

public record GetPropertyByIdQuery(Guid PropertyId) : IRequest<Result<PropertyDetailResponse>>;

using MediatR;
using SmartMetering.Contracts.Properties;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.GetProperties;

public record GetPropertiesQuery : IRequest<Result<List<PropertyResponse>>>;

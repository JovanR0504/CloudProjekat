using MediatR;
using SmartMetering.Contracts.TariffModels;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.GetTariffModels;

public record GetTariffModelsQuery : IRequest<Result<List<TariffModelResponse>>>;

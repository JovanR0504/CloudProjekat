using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.DeleteTariffModel;

public record DeleteTariffModelCommand(Guid Id) : IRequest<Result>;

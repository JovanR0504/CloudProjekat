using MediatR;
using SmartMetering.Contracts.TariffModels;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.UpdateTariffModel;

public record UpdateTariffModelCommand(
    Guid Id,
    string Name,
    decimal GreenHighTariffPrice,
    decimal GreenLowTariffPrice,
    decimal BlueHighTariffPrice,
    decimal BlueLowTariffPrice,
    decimal RedHighTariffPrice,
    decimal RedLowTariffPrice,
    decimal GreenZoneUpperLimit,
    decimal BlueZoneUpperLimit,
    decimal PowerBillingRate,
    decimal SupplierFee,
    string EffectiveFrom,
    string? EffectiveTo,
    bool IsActive) : IRequest<Result<TariffModelResponse>>;

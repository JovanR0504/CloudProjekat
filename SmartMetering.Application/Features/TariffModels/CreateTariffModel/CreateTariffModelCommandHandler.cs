using MediatR;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.TariffModels;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;

namespace SmartMetering.Application.Features.TariffModels.CreateTariffModel;

public class CreateTariffModelCommandHandler : IRequestHandler<CreateTariffModelCommand, Result<TariffModelResponse>>
{
    private readonly IAppDbContext _context;

    public CreateTariffModelCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TariffModelResponse>> Handle(CreateTariffModelCommand request, CancellationToken cancellationToken)
    {
        var effectiveFrom = DateOnly.Parse(request.EffectiveFrom);
        DateOnly? effectiveTo = request.EffectiveTo is not null
            ? DateOnly.Parse(request.EffectiveTo)
            : null;

        var tariffResult = TariffModel.Create(
            request.Name,
            request.GreenHighTariffPrice,
            request.GreenLowTariffPrice,
            request.BlueHighTariffPrice,
            request.BlueLowTariffPrice,
            request.RedHighTariffPrice,
            request.RedLowTariffPrice,
            request.GreenZoneUpperLimit,
            request.BlueZoneUpperLimit,
            request.PowerBillingRate,
            request.SupplierFee,
            effectiveFrom,
            effectiveTo);

        if (tariffResult.IsFailure)
            return Result<TariffModelResponse>.Failure(tariffResult.Error!);

        var tariff = tariffResult.Value;
        _context.TariffModels.Add(tariff);

        await _context.SaveChangesAsync(cancellationToken);

        var response = new TariffModelResponse(
            tariff.Id,
            tariff.Name,
            tariff.GreenHighTariffPrice,
            tariff.GreenLowTariffPrice,
            tariff.BlueHighTariffPrice,
            tariff.BlueLowTariffPrice,
            tariff.RedHighTariffPrice,
            tariff.RedLowTariffPrice,
            tariff.GreenZoneUpperLimit,
            tariff.BlueZoneUpperLimit,
            tariff.PowerBillingRate,
            tariff.SupplierFee,
            tariff.EffectiveFrom,
            tariff.EffectiveTo,
            tariff.IsActive,
            tariff.CreatedAtUtc);

        return Result<TariffModelResponse>.Success(response);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.TariffModels;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.UpdateTariffModel;

public class UpdateTariffModelCommandHandler : IRequestHandler<UpdateTariffModelCommand, Result<TariffModelResponse>>
{
    private readonly IAppDbContext _context;

    public UpdateTariffModelCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TariffModelResponse>> Handle(UpdateTariffModelCommand request, CancellationToken cancellationToken)
    {
        var tariff = await _context.TariffModels
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tariff is null)
            return Result<TariffModelResponse>.Failure(Error.NotFound("TariffModels.NotFound", "Tariff model not found."));

        var effectiveFrom = DateOnly.Parse(request.EffectiveFrom);
        DateOnly? effectiveTo = request.EffectiveTo is not null
            ? DateOnly.Parse(request.EffectiveTo)
            : null;

        tariff.Update(
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
            effectiveTo,
            request.IsActive);

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

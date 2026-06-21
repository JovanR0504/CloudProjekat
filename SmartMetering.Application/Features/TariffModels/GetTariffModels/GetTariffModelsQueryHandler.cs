using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.TariffModels;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.GetTariffModels;

public class GetTariffModelsQueryHandler : IRequestHandler<GetTariffModelsQuery, Result<List<TariffModelResponse>>>
{
    private readonly IAppDbContext _context;

    public GetTariffModelsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TariffModelResponse>>> Handle(GetTariffModelsQuery request, CancellationToken cancellationToken)
    {
        var tariffs = await _context.TariffModels
            .AsNoTracking()
            .OrderByDescending(t => t.EffectiveFrom)
            .Select(t => new TariffModelResponse(
                t.Id,
                t.Name,
                t.GreenHighTariffPrice,
                t.GreenLowTariffPrice,
                t.BlueHighTariffPrice,
                t.BlueLowTariffPrice,
                t.RedHighTariffPrice,
                t.RedLowTariffPrice,
                t.GreenZoneUpperLimit,
                t.BlueZoneUpperLimit,
                t.PowerBillingRate,
                t.SupplierFee,
                t.EffectiveFrom,
                t.EffectiveTo,
                t.IsActive,
                t.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result<List<TariffModelResponse>>.Success(tariffs);
    }
}

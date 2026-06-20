using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.ConsumptionLimits;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.ConsumptionLimits.GetLimit;

public class GetConsumptionLimitQueryHandler : IRequestHandler<GetConsumptionLimitQuery, Result<ConsumptionLimitResponse?>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetConsumptionLimitQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ConsumptionLimitResponse?>> Handle(GetConsumptionLimitQuery request, CancellationToken cancellationToken)
    {
        var limit = await _context.ConsumptionLimits
            .AsNoTracking()
            .FirstOrDefaultAsync(cl => cl.UserId == _currentUser.UserId, cancellationToken);

        if (limit is null)
            return Result<ConsumptionLimitResponse?>.Success(null);

        var response = new ConsumptionLimitResponse(
            limit.Id,
            limit.UserId,
            limit.Threshold,
            limit.Unit.ToString(),
            limit.NotifiedForMonth);

        return Result<ConsumptionLimitResponse?>.Success(response);
    }
}

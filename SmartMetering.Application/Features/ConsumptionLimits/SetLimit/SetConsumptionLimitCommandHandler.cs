using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.ConsumptionLimits;
using SmartMetering.Domain.Common;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Application.Features.ConsumptionLimits.SetLimit;

public class SetConsumptionLimitCommandHandler : IRequestHandler<SetConsumptionLimitCommand, Result<ConsumptionLimitResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SetConsumptionLimitCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ConsumptionLimitResponse>> Handle(SetConsumptionLimitCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<LimitUnit>(request.Unit, ignoreCase: true, out var unit))
            return Result<ConsumptionLimitResponse>.Failure(
                Error.Validation("ConsumptionLimit.InvalidUnit", "Unit must be 'Kwh' or 'Rsd'."));

        var existing = await _context.ConsumptionLimits
            .FirstOrDefaultAsync(cl => cl.UserId == _currentUser.UserId, cancellationToken);

        ConsumptionLimit limit;

        if (existing is not null)
        {
            existing.UpdateThreshold(request.Threshold, unit);
            limit = existing;
        }
        else
        {
            var createResult = ConsumptionLimit.Create(_currentUser.UserId, request.Threshold, unit);

            if (createResult.IsFailure)
                return Result<ConsumptionLimitResponse>.Failure(createResult.Error!);

            limit = createResult.Value;
            _context.ConsumptionLimits.Add(limit);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var response = new ConsumptionLimitResponse(
            limit.Id,
            limit.UserId,
            limit.Threshold,
            limit.Unit.ToString(),
            limit.NotifiedForMonth);

        return Result<ConsumptionLimitResponse>.Success(response);
    }
}

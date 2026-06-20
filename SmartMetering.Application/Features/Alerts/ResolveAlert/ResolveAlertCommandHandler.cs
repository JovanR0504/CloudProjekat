using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Alerts.ResolveAlert;

public class ResolveAlertCommandHandler : IRequestHandler<ResolveAlertCommand, Result>
{
    private readonly IAppDbContext _context;

    public ResolveAlertCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(ResolveAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _context.Alerts
            .FirstOrDefaultAsync(a => a.Id == request.AlertId, cancellationToken);

        if (alert is null)
            return Result.Failure(Error.NotFound("Alerts.NotFound", "Alert not found."));

        alert.Resolve();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

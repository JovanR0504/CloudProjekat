using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.TariffModels.DeleteTariffModel;

public class DeleteTariffModelCommandHandler : IRequestHandler<DeleteTariffModelCommand, Result>
{
    private readonly IAppDbContext _context;

    public DeleteTariffModelCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTariffModelCommand request, CancellationToken cancellationToken)
    {
        var tariff = await _context.TariffModels
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tariff is null)
            return Result.Failure(Error.NotFound("TariffModels.NotFound", "Tariff model not found."));

        _context.TariffModels.Remove(tariff);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

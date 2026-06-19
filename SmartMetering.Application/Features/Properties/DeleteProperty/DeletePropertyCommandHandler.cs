using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.DeleteProperty;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Result>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeletePropertyCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure(Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result.Failure(Error.Unauthorized("Properties.Forbidden", "You do not own this property."));

        _context.Properties.Remove(property);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

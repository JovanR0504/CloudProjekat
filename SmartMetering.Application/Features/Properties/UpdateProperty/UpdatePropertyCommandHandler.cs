using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Properties.UpdateProperty;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdatePropertyCommandHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == request.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure(Error.NotFound("Properties.NotFound", "Property not found."));

        if (property.UserId != _currentUser.UserId)
            return Result.Failure(Error.Unauthorized("Properties.Forbidden", "You do not own this property."));

        property.Update(request.Name, request.City, request.Address, request.Description);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

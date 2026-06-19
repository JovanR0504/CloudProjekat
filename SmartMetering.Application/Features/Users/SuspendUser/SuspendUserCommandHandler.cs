using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Users.SuspendUser;

public class SuspendUserCommandHandler : IRequestHandler<SuspendUserCommand, Result>
{
    private readonly IAppDbContext _context;

    public SuspendUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound("Users.NotFound", "User not found."));

        user.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

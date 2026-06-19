using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Auth;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserQueryHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
            return Result<CurrentUserResponse>.Failure(
                Error.NotFound("Auth.UserNotFound", "User not found."));

        var response = new CurrentUserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString());

        return Result<CurrentUserResponse>.Success(response);
    }
}

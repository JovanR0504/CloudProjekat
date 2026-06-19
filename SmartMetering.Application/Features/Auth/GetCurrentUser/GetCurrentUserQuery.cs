using MediatR;
using SmartMetering.Contracts.Auth;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<CurrentUserResponse>>;

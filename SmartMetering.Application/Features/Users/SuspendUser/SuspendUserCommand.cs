using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Users.SuspendUser;

public record SuspendUserCommand(Guid UserId) : IRequest<Result>;

using MediatR;
using SmartMetering.Contracts.Users;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Users.GetUsers;

public record GetUsersQuery : IRequest<Result<List<UserResponse>>>;

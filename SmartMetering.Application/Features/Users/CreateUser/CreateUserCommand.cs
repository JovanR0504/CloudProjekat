using MediatR;
using SmartMetering.Contracts.Users;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Users.CreateUser;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role) : IRequest<Result<UserResponse>>;

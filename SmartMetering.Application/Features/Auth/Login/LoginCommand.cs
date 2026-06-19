using MediatR;
using SmartMetering.Contracts.Auth;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

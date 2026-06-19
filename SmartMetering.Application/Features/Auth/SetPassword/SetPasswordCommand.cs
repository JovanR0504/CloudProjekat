using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.SetPassword;

public record SetPasswordCommand(string Token, string Password, string ConfirmPassword) : IRequest<Result>;

using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

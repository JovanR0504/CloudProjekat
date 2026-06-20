using MediatR;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Alerts.ResolveAlert;

public record ResolveAlertCommand(Guid AlertId) : IRequest<Result>;

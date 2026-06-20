using MediatR;
using SmartMetering.Contracts.Alerts;
using SmartMetering.Domain.Common;

namespace SmartMetering.Application.Features.Alerts.GetAlerts;

public record GetAlertsQuery : IRequest<Result<List<AlertResponse>>>;

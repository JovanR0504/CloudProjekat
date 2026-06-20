using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Alerts.GetAlerts;
using SmartMetering.Application.Features.Alerts.ResolveAlert;

namespace SmartMetering.API.Controllers;

[Authorize]
public class AlertsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAlertsQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var command = new ResolveAlertCommand(id);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

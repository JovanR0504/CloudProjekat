using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Telemetry.GetDailyConsumption;
using SmartMetering.Application.Features.Telemetry.GetTelemetryTrend;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Consumer")]
[Route("api/properties/{propertyId:guid}/telemetry")]
public class TelemetryController : BaseApiController
{
    private readonly IMediator _mediator;

    public TelemetryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyConsumption(
        Guid propertyId,
        [FromQuery] DateOnly? date)
    {
        var query = new GetDailyConsumptionQuery(propertyId, date);
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetTelemetryTrend(
        Guid propertyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var query = new GetTelemetryTrendQuery(propertyId, from, to);
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}

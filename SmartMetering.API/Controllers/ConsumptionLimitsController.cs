using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.ConsumptionLimits.GetLimit;
using SmartMetering.Application.Features.ConsumptionLimits.SetLimit;
using SmartMetering.Contracts.ConsumptionLimits;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Consumer")]
[Route("api/consumption-limit")]
public class ConsumptionLimitsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ConsumptionLimitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var query = new GetConsumptionLimitQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Set([FromBody] SetConsumptionLimitRequest request)
    {
        var command = new SetConsumptionLimitCommand(request.Threshold, request.Unit);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

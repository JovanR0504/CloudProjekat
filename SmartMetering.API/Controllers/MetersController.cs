using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Meters.CreateMeter;
using SmartMetering.Application.Features.Meters.DeleteMeter;
using SmartMetering.Application.Features.Meters.GetMeterById;
using SmartMetering.Application.Features.Meters.RegisterDevice;
using SmartMetering.Application.Features.Meters.UpdateMeter;
using SmartMetering.Contracts.Meters;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Consumer")]
public class MetersController : BaseApiController
{
    private readonly IMediator _mediator;

    public MetersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeterRequest request)
    {
        var command = new CreateMeterCommand(
            request.PropertyId,
            request.Label,
            request.SerialNumber,
            request.ConnectionType,
            request.Notes);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetMeterByIdQuery(id);
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMeterRequest request)
    {
        var command = new UpdateMeterCommand(
            id,
            request.Label,
            request.Notes);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteMeterCommand(id);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("register-device")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        var command = new RegisterDeviceCommand(
            request.DeviceId,
            request.SerialNumber);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

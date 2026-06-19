using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Properties.CreateProperty;
using SmartMetering.Application.Features.Properties.DeleteProperty;
using SmartMetering.Application.Features.Properties.GetProperties;
using SmartMetering.Application.Features.Properties.GetPropertyById;
using SmartMetering.Application.Features.Properties.UpdateProperty;
using SmartMetering.Contracts.Properties;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Consumer")]
public class PropertiesController : BaseApiController
{
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var command = new CreatePropertyCommand(
            request.Name,
            request.City,
            request.Address,
            request.Description);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetPropertiesQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPropertyByIdQuery(id);
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var command = new UpdatePropertyCommand(
            id,
            request.Name,
            request.City,
            request.Address,
            request.Description);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeletePropertyCommand(id);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

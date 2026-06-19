using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMetering.Application.Features.Users.CreateUser;
using SmartMetering.Application.Features.Users.GetUsers;
using SmartMetering.Application.Features.Users.SuspendUser;
using SmartMetering.Contracts.Users;

namespace SmartMetering.API.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Role);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        var command = new SuspendUserCommand(id);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

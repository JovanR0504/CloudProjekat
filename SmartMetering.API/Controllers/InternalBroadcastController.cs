using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartMetering.API.Hubs;
using SmartMetering.Contracts.Telemetry;

namespace SmartMetering.API.Controllers;

[ApiController]
[Route("api/internal/broadcast")]
public class InternalBroadcastController : ControllerBase
{
    private readonly IHubContext<TelemetryHub> _hubContext;
    private readonly IConfiguration _configuration;

    public InternalBroadcastController(
        IHubContext<TelemetryHub> hubContext,
        IConfiguration configuration)
    {
        _hubContext = hubContext;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Broadcast([FromBody] TelemetryBroadcastEvent evt)
    {
        var expectedSecret = _configuration["Broadcast:SharedSecret"];

        if (string.IsNullOrEmpty(expectedSecret))
            return StatusCode(500, new { Message = "Broadcast secret not configured." });

        var providedSecret = Request.Headers["X-Broadcast-Secret"].FirstOrDefault();

        if (string.IsNullOrEmpty(providedSecret) || providedSecret != expectedSecret)
            return Unauthorized(new { Message = "Invalid broadcast secret." });

        var groupName = $"property-{evt.PropertyId}";

        await _hubContext.Clients.Group(groupName)
            .SendAsync("ReceiveTelemetry", evt);

        return Ok();
    }
}

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Functions;

public class ReceiveTelemetryFunction
{
    private readonly IAppDbContext _db;
    private readonly QueueClient _queueClient;
    private readonly ILogger<ReceiveTelemetryFunction> _logger;

    public ReceiveTelemetryFunction(
        IAppDbContext db,
        QueueClient queueClient,
        ILogger<ReceiveTelemetryFunction> logger)
    {
        _db = db;
        _queueClient = queueClient;
        _logger = logger;
    }

    [Function("ReceiveTelemetry")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/telemetry")]
        HttpRequest req)
    {
        var deviceId = req.Headers["X-Device-Id"].FirstOrDefault();
        var deviceToken = req.Headers["X-Device-Token"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(deviceId) || string.IsNullOrWhiteSpace(deviceToken))
        {
            _logger.LogWarning("Missing device headers.");
            return new UnauthorizedResult();
        }

        var meter = await _db.Meters
            .FirstOrDefaultAsync(m => m.DeviceId == deviceId);

        if (meter is null)
        {
            _logger.LogWarning("Meter not found for DeviceId {DeviceId}.", deviceId);
            return new UnauthorizedResult();
        }

        if (meter.Status != MeterStatus.Active && meter.Status != MeterStatus.Registered)
        {
            _logger.LogWarning("Meter {MeterId} has invalid status {Status}.", meter.Id, meter.Status);
            return new UnauthorizedResult();
        }

        // Constant-time token comparison
        var presentedHash = SHA256.HashData(Encoding.UTF8.GetBytes(deviceToken));
        var storedHash = Convert.FromHexString(meter.DeviceTokenHash!);

        if (!CryptographicOperations.FixedTimeEquals(presentedHash, storedHash))
        {
            _logger.LogWarning("Invalid device token for meter {MeterId}.", meter.Id);
            return new UnauthorizedResult();
        }

        using var reader = new StreamReader(req.Body);
        var body = await reader.ReadToEndAsync();

        var payload = JsonSerializer.Deserialize<TelemetryPayload>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (payload is null)
        {
            _logger.LogWarning("Invalid telemetry payload.");
            return new BadRequestResult();
        }

        var queueItem = new TelemetryQueueItem(
            meter.Id,
            payload.Voltage,
            payload.Current,
            payload.PowerFactor,
            payload.TotalKwh,
            payload.LoadKw,
            payload.RecordedAtUtc);

        var message = JsonSerializer.Serialize(queueItem);
        await _queueClient.SendMessageAsync(message);

        _logger.LogInformation("Telemetry enqueued for meter {MeterId}.", meter.Id);

        return new AcceptedResult();
    }
}

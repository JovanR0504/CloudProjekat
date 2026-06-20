using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Helpers;

namespace SmartMetering.Functions;

public class ProcessTelemetryFunction
{
    private readonly ITelemetryStore _telemetryStore;
    private readonly IAppDbContext _db;
    private readonly ITelemetryBroadcaster _broadcaster;
    private readonly ILogger<ProcessTelemetryFunction> _logger;

    public ProcessTelemetryFunction(
        ITelemetryStore telemetryStore,
        IAppDbContext db,
        ITelemetryBroadcaster broadcaster,
        ILogger<ProcessTelemetryFunction> logger)
    {
        _telemetryStore = telemetryStore;
        _db = db;
        _broadcaster = broadcaster;
        _logger = logger;
    }

    [Function("ProcessTelemetry")]
    public async Task Run(
        [QueueTrigger("telemetry-queue", Connection = "ConnectionStrings:AzureStorage")]
        string message)
    {
        var queueItem = JsonSerializer.Deserialize<TelemetryQueueItem>(message,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (queueItem is null)
        {
            _logger.LogError("Failed to deserialize queue message.");
            return;
        }

        var tariffType = TariffHelper.Classify(queueItem.RecordedAtUtc);

        var telemetry = new Telemetry
        {
            MeterId = queueItem.MeterId,
            Voltage = queueItem.Voltage,
            Current = queueItem.Current,
            PowerFactor = queueItem.PowerFactor,
            TotalKwh = queueItem.TotalKwh,
            LoadKw = queueItem.LoadKw,
            RecordedAtUtc = queueItem.RecordedAtUtc,
            TariffType = tariffType
        };

        await _telemetryStore.SaveAsync(telemetry);

        var meter = await _db.Meters
            .FirstOrDefaultAsync(m => m.Id == queueItem.MeterId);

        if (meter is not null)
        {
            meter.MarkActive(queueItem.RecordedAtUtc);
            await _db.SaveChangesAsync();

            var broadcastEvent = new TelemetryBroadcastEvent(
                queueItem.MeterId.ToString(),
                meter.PropertyId.ToString(),
                queueItem.RecordedAtUtc,
                queueItem.Voltage,
                queueItem.LoadKw,
                tariffType.ToString(),
                queueItem.TotalKwh);

            await _broadcaster.BroadcastAsync(broadcastEvent);
        }

        _logger.LogInformation(
            "Telemetry processed for meter {MeterId}. Tariff: {Tariff}, LoadKw: {LoadKw:F2}.",
            queueItem.MeterId, tariffType, queueItem.LoadKw);
    }
}

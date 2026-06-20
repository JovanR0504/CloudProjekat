using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;
using SmartMetering.Domain.Helpers;

namespace SmartMetering.Functions;

public class ProcessTelemetryFunction
{
    private readonly ITelemetryStore _telemetryStore;
    private readonly IAppDbContext _db;
    private readonly ITelemetryBroadcaster _broadcaster;
    private readonly IEmailService _emailService;
    private readonly ILogger<ProcessTelemetryFunction> _logger;

    private const double VoltageDropThreshold = 190.0;
    private const decimal RsdPerKwhRate = 7.5m;

    public ProcessTelemetryFunction(
        ITelemetryStore telemetryStore,
        IAppDbContext db,
        ITelemetryBroadcaster broadcaster,
        IEmailService emailService,
        ILogger<ProcessTelemetryFunction> logger)
    {
        _telemetryStore = telemetryStore;
        _db = db;
        _broadcaster = broadcaster;
        _emailService = emailService;
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

            // Voltage drop detection
            if (queueItem.Voltage < VoltageDropThreshold)
            {
                await HandleVoltageDrop(queueItem, meter);
            }

            // Consumption limit check
            await CheckConsumptionLimit(queueItem, meter);
        }

        _logger.LogInformation(
            "Telemetry processed for meter {MeterId}. Tariff: {Tariff}, LoadKw: {LoadKw:F2}.",
            queueItem.MeterId, tariffType, queueItem.LoadKw);
    }

    private async Task HandleVoltageDrop(TelemetryQueueItem queueItem, Meter meter)
    {
        var adminUsers = await _db.Users
            .Where(u => u.Role == UserRole.Admin)
            .ToListAsync();

        foreach (var admin in adminUsers)
        {
            var alertResult = Alert.Create(
                AlertType.LowVoltage,
                AlertSeverity.Critical,
                admin.Id,
                $"Low voltage detected: {queueItem.Voltage:F1}V on meter '{meter.Label}' (SN: {meter.SerialNumber}).",
                meterId: meter.Id,
                propertyId: meter.PropertyId);

            if (alertResult.IsSuccess)
            {
                _db.Alerts.Add(alertResult.Value);
            }
        }

        await _db.SaveChangesAsync();

        _logger.LogWarning(
            "Voltage drop detected on meter {MeterId}: {Voltage}V (threshold: {Threshold}V).",
            queueItem.MeterId, queueItem.Voltage, VoltageDropThreshold);
    }

    private async Task CheckConsumptionLimit(TelemetryQueueItem queueItem, Meter meter)
    {
        var property = await _db.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == meter.PropertyId);

        if (property is null)
            return;

        var owner = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == property.UserId);

        if (owner is null)
            return;

        var limit = await _db.ConsumptionLimits
            .FirstOrDefaultAsync(cl => cl.UserId == owner.Id);

        if (limit is null)
            return;

        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");

        if (limit.NotifiedForMonth == currentMonth)
            return;

        var now = DateTime.UtcNow;
        var (highTariffKwh, lowTariffKwh) = await _telemetryStore.GetMonthlyConsumptionAsync(
            meter.Id, now.Month, now.Year);

        var monthlyKwh = highTariffKwh + lowTariffKwh;
        decimal consumption;
        string unitLabel;

        if (limit.Unit == LimitUnit.Rsd)
        {
            consumption = (decimal)monthlyKwh * RsdPerKwhRate;
            unitLabel = "RSD";
        }
        else
        {
            consumption = (decimal)monthlyKwh;
            unitLabel = "kWh";
        }

        if (consumption >= limit.Threshold)
        {
            await _emailService.SendLimitExceededEmailAsync(
                owner.Email,
                owner.FirstName,
                consumption,
                limit.Threshold,
                unitLabel);

            limit.RecordNotification(currentMonth);

            var alertResult = Alert.Create(
                AlertType.ConsumptionLimitExceeded,
                AlertSeverity.Warning,
                owner.Id,
                $"Monthly consumption ({consumption:F2} {unitLabel}) exceeded limit ({limit.Threshold:F2} {unitLabel}).",
                meterId: meter.Id,
                propertyId: meter.PropertyId);

            if (alertResult.IsSuccess)
            {
                _db.Alerts.Add(alertResult.Value);
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Consumption limit exceeded for user {UserId}. Usage: {Usage:F2} {Unit}, Limit: {Limit:F2} {Unit}.",
                owner.Id, consumption, unitLabel, limit.Threshold, unitLabel);
        }
    }
}

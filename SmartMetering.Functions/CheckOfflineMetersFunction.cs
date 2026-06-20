using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Functions;

public class CheckOfflineMetersFunction
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CheckOfflineMetersFunction> _logger;

    public CheckOfflineMetersFunction(
        IAppDbContext db,
        ILogger<CheckOfflineMetersFunction> logger)
    {
        _db = db;
        _logger = logger;
    }

    [Function("CheckOfflineMeters")]
    public async Task Run(
        [TimerTrigger("*/30 * * * * *")] TimerInfo timerInfo)
    {
        var threshold = DateTime.UtcNow.AddMinutes(-3);

        var offlineMeters = await _db.Meters
            .Where(m => m.Status == MeterStatus.Active)
            .Where(m => m.LastActiveAtUtc != null && m.LastActiveAtUtc < threshold)
            .ToListAsync();

        if (offlineMeters.Count == 0)
        {
            _logger.LogDebug("No offline meters detected.");
            return;
        }

        // Find an admin user for alert assignment
        var adminUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.Admin && u.IsActive);

        foreach (var meter in offlineMeters)
        {
            meter.MarkInactive();

            if (adminUser is not null)
            {
                // Look up the property to include in the alert
                var property = await _db.Properties
                    .FirstOrDefaultAsync(p => p.Id == meter.PropertyId);

                var alertResult = Alert.Create(
                    AlertType.DeviceOffline,
                    AlertSeverity.Warning,
                    adminUser.Id,
                    $"Meter '{meter.Label}' (SN: {meter.SerialNumber}) went offline. " +
                    $"Last active: {meter.LastActiveAtUtc:yyyy-MM-dd HH:mm:ss} UTC.",
                    meterId: meter.Id,
                    propertyId: property?.Id);

                if (alertResult.IsSuccess)
                {
                    _db.Alerts.Add(alertResult.Value);
                }
            }

            _logger.LogWarning(
                "Meter {MeterId} ({Label}) marked inactive. Last active: {LastActive}.",
                meter.Id, meter.Label, meter.LastActiveAtUtc);
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Marked {Count} meter(s) as inactive.", offlineMeters.Count);
    }
}

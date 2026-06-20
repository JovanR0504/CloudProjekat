using SmartMetering.Contracts.Telemetry;

namespace SmartMetering.Application.Interfaces;

public interface ITelemetryBroadcaster
{
    Task BroadcastAsync(TelemetryBroadcastEvent evt, CancellationToken ct = default);
}

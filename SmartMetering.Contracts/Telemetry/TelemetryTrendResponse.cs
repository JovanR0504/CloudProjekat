namespace SmartMetering.Contracts.Telemetry;

public record TelemetryTrendResponse(
    Guid PropertyId,
    DateTime FromUtc,
    DateTime ToUtc,
    List<TelemetrySample> Samples);

public record TelemetrySample(
    Guid MeterId,
    DateTime RecordedAtUtc,
    double Voltage,
    double LoadKw);

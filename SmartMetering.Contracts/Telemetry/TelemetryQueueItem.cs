namespace SmartMetering.Contracts.Telemetry;

public record TelemetryQueueItem(
    Guid MeterId,
    double Voltage,
    double Current,
    double PowerFactor,
    double TotalKwh,
    double LoadKw,
    DateTime RecordedAtUtc);

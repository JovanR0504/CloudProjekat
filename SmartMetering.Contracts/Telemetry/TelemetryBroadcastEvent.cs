namespace SmartMetering.Contracts.Telemetry;

public record TelemetryBroadcastEvent(
    string MeterId,
    string PropertyId,
    DateTime RecordedAtUtc,
    double Voltage,
    double LoadKw,
    string Tariff,
    double TotalKwh);

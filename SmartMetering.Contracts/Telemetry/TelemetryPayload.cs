namespace SmartMetering.Contracts.Telemetry;

public record TelemetryPayload(
    double Voltage,
    double Current,
    double PowerFactor,
    double TotalKwh,
    double LoadKw,
    DateTime RecordedAtUtc);

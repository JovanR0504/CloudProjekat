namespace SmartMetering.Contracts.Telemetry;

public record DailyConsumptionResponse(
    Guid PropertyId,
    string FromDate,
    string ToDate,
    List<DailyConsumptionPoint> Points);

public record DailyConsumptionPoint(
    string Date,
    double VtKwh,
    double NtKwh);

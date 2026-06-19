namespace SmartMetering.Contracts.Meters;

public record MeterResponse(
    Guid Id,
    Guid PropertyId,
    string Label,
    string SerialNumber,
    string ConnectionType,
    double ApprovedPowerKw,
    string Status,
    string? Notes,
    string? DeviceId,
    DateTime CreatedAtUtc);

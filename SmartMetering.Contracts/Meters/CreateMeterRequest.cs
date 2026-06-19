namespace SmartMetering.Contracts.Meters;

public record CreateMeterRequest(
    Guid PropertyId,
    string Label,
    string SerialNumber,
    string ConnectionType,
    string? Notes);

namespace SmartMetering.Contracts.Meters;

public record UpdateMeterRequest(
    string Label,
    string? Notes);

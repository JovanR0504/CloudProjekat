namespace SmartMetering.Contracts.Meters;

public record RegisterDeviceRequest(
    string DeviceId,
    string SerialNumber);

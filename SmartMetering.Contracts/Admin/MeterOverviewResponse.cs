namespace SmartMetering.Contracts.Admin;

public record MeterOverviewResponse(
    Guid Id,
    string Label,
    string SerialNumber,
    string Status,
    string ConnectionType,
    double ApprovedPowerKw,
    string? DeviceId,
    string PropertyName,
    string PropertyCity,
    string PropertyAddress,
    string OwnerName,
    string OwnerEmail);

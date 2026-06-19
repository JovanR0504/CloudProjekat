namespace SmartMetering.Contracts.Properties;

public record PropertyDetailResponse(
    Guid Id,
    string Name,
    string City,
    string Address,
    string? Description,
    Guid UserId,
    DateTime CreatedAtUtc,
    List<MeterSummary> Meters);

public record MeterSummary(
    Guid Id,
    string Label,
    string SerialNumber,
    string ConnectionType,
    double ApprovedPowerKw,
    string Status);

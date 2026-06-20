namespace SmartMetering.Contracts.ConsumptionLimits;

public record ConsumptionLimitResponse(
    Guid Id,
    Guid UserId,
    decimal Threshold,
    string Unit,
    string? NotifiedForMonth);

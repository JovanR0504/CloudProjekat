namespace SmartMetering.Contracts.ConsumptionLimits;

public record SetConsumptionLimitRequest(
    decimal Threshold,
    string Unit);

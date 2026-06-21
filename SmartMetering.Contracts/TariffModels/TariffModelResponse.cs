namespace SmartMetering.Contracts.TariffModels;

public record TariffModelResponse(
    Guid Id,
    string Name,
    decimal GreenHighTariffPrice,
    decimal GreenLowTariffPrice,
    decimal BlueHighTariffPrice,
    decimal BlueLowTariffPrice,
    decimal RedHighTariffPrice,
    decimal RedLowTariffPrice,
    decimal GreenZoneUpperLimit,
    decimal BlueZoneUpperLimit,
    decimal PowerBillingRate,
    decimal SupplierFee,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsActive,
    DateTime CreatedAtUtc);

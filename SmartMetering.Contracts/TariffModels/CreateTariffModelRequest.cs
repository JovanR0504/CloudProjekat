namespace SmartMetering.Contracts.TariffModels;

public record CreateTariffModelRequest(
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
    string EffectiveFrom,
    string? EffectiveTo,
    bool IsActive);

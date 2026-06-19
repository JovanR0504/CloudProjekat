using SmartMetering.Domain.Common;

namespace SmartMetering.Domain.Entities;

public class TariffModel : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public decimal GreenHighTariffPrice { get; private set; }
    public decimal GreenLowTariffPrice { get; private set; }
    public decimal BlueHighTariffPrice { get; private set; }
    public decimal BlueLowTariffPrice { get; private set; }
    public decimal RedHighTariffPrice { get; private set; }
    public decimal RedLowTariffPrice { get; private set; }
    public decimal GreenZoneUpperLimit { get; private set; } = 350m;
    public decimal BlueZoneUpperLimit { get; private set; } = 1200m;
    public decimal PowerBillingRate { get; private set; }
    public decimal SupplierFee { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }

    private TariffModel() { }

    public static Result<TariffModel> Create(
        string name,
        decimal greenHighTariffPrice,
        decimal greenLowTariffPrice,
        decimal blueHighTariffPrice,
        decimal blueLowTariffPrice,
        decimal redHighTariffPrice,
        decimal redLowTariffPrice,
        decimal greenZoneUpperLimit,
        decimal blueZoneUpperLimit,
        decimal powerBillingRate,
        decimal supplierFee,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<TariffModel>.Failure(Error.Validation("TariffModel.EmptyName", "Tariff model name is required."));

        if (greenZoneUpperLimit <= 0)
            return Result<TariffModel>.Failure(Error.Validation("TariffModel.InvalidGreenLimit", "Green zone upper limit must be positive."));

        if (blueZoneUpperLimit <= greenZoneUpperLimit)
            return Result<TariffModel>.Failure(Error.Validation("TariffModel.InvalidBlueLimit", "Blue zone upper limit must be greater than green zone upper limit."));

        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
            return Result<TariffModel>.Failure(Error.Validation("TariffModel.InvalidDateRange", "Effective end date must be after start date."));

        var tariff = new TariffModel
        {
            Name = name.Trim(),
            GreenHighTariffPrice = greenHighTariffPrice,
            GreenLowTariffPrice = greenLowTariffPrice,
            BlueHighTariffPrice = blueHighTariffPrice,
            BlueLowTariffPrice = blueLowTariffPrice,
            RedHighTariffPrice = redHighTariffPrice,
            RedLowTariffPrice = redLowTariffPrice,
            GreenZoneUpperLimit = greenZoneUpperLimit,
            BlueZoneUpperLimit = blueZoneUpperLimit,
            PowerBillingRate = powerBillingRate,
            SupplierFee = supplierFee,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            IsActive = true
        };

        return Result<TariffModel>.Success(tariff);
    }

    public void Deactivate()
    {
        IsActive = false;
        EffectiveTo = DateOnly.FromDateTime(DateTime.UtcNow);
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

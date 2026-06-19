using SmartMetering.Domain.Entities;

namespace SmartMetering.Application.Interfaces;

public interface ITelemetryStore
{
    Task SaveAsync(Telemetry telemetry);
    Task<IReadOnlyList<Telemetry>> QueryAsync(Guid meterId, DateTime from, DateTime to);
    Task<(decimal highTariffKwh, decimal lowTariffKwh)> GetMonthlyConsumptionAsync(Guid meterId, int month, int year);
}

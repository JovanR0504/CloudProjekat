using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using SmartMetering.Application.Interfaces;
using SmartMetering.Domain.Entities;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Infrastructure.Services;

public class TelemetryTableStore : ITelemetryStore
{
    private const string TableName = "Telemetry";
    private readonly TableClient _tableClient;

    public TelemetryTableStore(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage")!;
        var serviceClient = new TableServiceClient(connectionString);
        _tableClient = serviceClient.GetTableClient(TableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task SaveAsync(Telemetry telemetry)
    {
        var entity = new TableEntity(
            telemetry.MeterId.ToString(),
            GetReverseTick(telemetry.RecordedAtUtc))
        {
            { "Voltage", telemetry.Voltage },
            { "Current", telemetry.Current },
            { "PowerFactor", telemetry.PowerFactor },
            { "TotalKwh", telemetry.TotalKwh },
            { "LoadKw", telemetry.LoadKw },
            { "RecordedAtUtc", telemetry.RecordedAtUtc },
            { "TariffType", (int)telemetry.TariffType }
        };

        await _tableClient.UpsertEntityAsync(entity);
    }

    public async Task<IReadOnlyList<Telemetry>> QueryAsync(Guid meterId, DateTime from, DateTime to)
    {
        var partitionKey = meterId.ToString();
        var rowKeyFrom = GetReverseTick(to);
        var rowKeyTo = GetReverseTick(from);

        var filter = $"PartitionKey eq '{partitionKey}' and RowKey ge '{rowKeyFrom}' and RowKey le '{rowKeyTo}'";

        var results = new List<Telemetry>();

        await foreach (var entity in _tableClient.QueryAsync<TableEntity>(filter))
        {
            results.Add(MapToTelemetry(meterId, entity));
        }

        return results.AsReadOnly();
    }

    public async Task<(decimal highTariffKwh, decimal lowTariffKwh)> GetMonthlyConsumptionAsync(
        Guid meterId, int month, int year)
    {
        var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1);

        var readings = await QueryAsync(meterId, from, to);

        if (readings.Count < 2)
            return (0m, 0m);

        var ordered = readings.OrderBy(r => r.RecordedAtUtc).ToList();

        decimal highKwh = 0m;
        decimal lowKwh = 0m;

        for (int i = 1; i < ordered.Count; i++)
        {
            var delta = (decimal)(ordered[i].TotalKwh - ordered[i - 1].TotalKwh);
            if (delta < 0) continue;

            if (ordered[i].TariffType == TariffType.HighTariff)
                highKwh += delta;
            else
                lowKwh += delta;
        }

        return (highKwh, lowKwh);
    }

    private static string GetReverseTick(DateTime utc)
    {
        return (DateTime.MaxValue.Ticks - utc.Ticks).ToString("D19");
    }

    private static Telemetry MapToTelemetry(Guid meterId, TableEntity entity)
    {
        return new Telemetry
        {
            MeterId = meterId,
            Voltage = entity.GetDouble("Voltage") ?? 0,
            Current = entity.GetDouble("Current") ?? 0,
            PowerFactor = entity.GetDouble("PowerFactor") ?? 0,
            TotalKwh = entity.GetDouble("TotalKwh") ?? 0,
            LoadKw = entity.GetDouble("LoadKw") ?? 0,
            RecordedAtUtc = entity.GetDateTime("RecordedAtUtc") ?? DateTime.UtcNow,
            TariffType = (TariffType)(entity.GetInt32("TariffType") ?? 0)
        };
    }
}

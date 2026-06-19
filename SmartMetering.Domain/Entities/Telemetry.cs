using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class Telemetry
{
    public Guid MeterId { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double PowerFactor { get; set; }
    public double TotalKwh { get; set; }
    public double LoadKw { get; set; }
    public DateTime RecordedAtUtc { get; set; }
    public TariffType TariffType { get; set; }
}

using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Helpers;

public static class TariffHelper
{
    private static readonly TimeZoneInfo BelgradeTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    public static TariffType Classify(DateTime utc)
    {
        var belgradeTime = TimeZoneInfo.ConvertTimeFromUtc(utc, BelgradeTimeZone);
        var hour = belgradeTime.Hour;

        // 07:00 - 23:00 = HighTariff, 23:00 - 07:00 = LowTariff
        return hour >= 7 && hour < 23 ? TariffType.HighTariff : TariffType.LowTariff;
    }
}

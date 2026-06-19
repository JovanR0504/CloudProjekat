using System.Text.RegularExpressions;

namespace SmartMetering.Domain.Helpers;

public static partial class SerialNumberValidator
{
    // Format: SM-YYYY-XXXXX where YYYY is a 4-digit year and XXXXX is 5 digits
    private static readonly Regex SerialPattern = new(
        @"^SM-\d{4}-\d{5}$",
        RegexOptions.Compiled);

    public static bool IsValid(string? serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return false;

        return SerialPattern.IsMatch(serialNumber.Trim().ToUpperInvariant());
    }
}

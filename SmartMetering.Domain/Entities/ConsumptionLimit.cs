using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class ConsumptionLimit : AggregateRoot
{
    public Guid UserId { get; private set; }
    public decimal Threshold { get; private set; }
    public LimitUnit Unit { get; private set; }
    public string? NotifiedForMonth { get; private set; }

    private ConsumptionLimit() { }

    public static Result<ConsumptionLimit> Create(
        Guid userId,
        decimal threshold,
        LimitUnit unit)
    {
        if (userId == Guid.Empty)
            return Result<ConsumptionLimit>.Failure(Error.Validation("ConsumptionLimit.EmptyUserId", "User ID is required."));

        if (threshold <= 0)
            return Result<ConsumptionLimit>.Failure(Error.Validation("ConsumptionLimit.InvalidThreshold", "Threshold must be a positive number."));

        var limit = new ConsumptionLimit
        {
            UserId = userId,
            Threshold = threshold,
            Unit = unit
        };

        return Result<ConsumptionLimit>.Success(limit);
    }

    public void RecordNotification(string month)
    {
        NotifiedForMonth = month;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateThreshold(decimal threshold, LimitUnit unit)
    {
        Threshold = threshold;
        Unit = unit;
        NotifiedForMonth = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

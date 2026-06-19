using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class Alert : AggregateRoot
{
    public AlertType AlertType { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public Guid? MeterId { get; private set; }
    public Guid? PropertyId { get; private set; }
    public Guid UserId { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }

    private Alert() { }

    public static Result<Alert> Create(
        AlertType alertType,
        AlertSeverity severity,
        Guid userId,
        string description,
        Guid? meterId = null,
        Guid? propertyId = null)
    {
        if (userId == Guid.Empty)
            return Result<Alert>.Failure(Error.Validation("Alert.EmptyUserId", "User ID is required."));

        if (string.IsNullOrWhiteSpace(description))
            return Result<Alert>.Failure(Error.Validation("Alert.EmptyDescription", "Description is required."));

        var alert = new Alert
        {
            AlertType = alertType,
            Severity = severity,
            UserId = userId,
            Description = description.Trim(),
            MeterId = meterId,
            PropertyId = propertyId,
            OccurredAtUtc = DateTime.UtcNow
        };

        return Result<Alert>.Success(alert);
    }

    public void Resolve()
    {
        ResolvedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

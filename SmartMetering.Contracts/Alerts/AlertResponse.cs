namespace SmartMetering.Contracts.Alerts;

public record AlertResponse(
    Guid Id,
    string AlertType,
    string Severity,
    Guid? MeterId,
    Guid? PropertyId,
    Guid UserId,
    string Description,
    DateTime OccurredAtUtc,
    DateTime? ResolvedAtUtc);

namespace SmartMetering.Contracts.Properties;

public record PropertyResponse(
    Guid Id,
    string Name,
    string City,
    string Address,
    string? Description,
    Guid UserId,
    DateTime CreatedAtUtc);

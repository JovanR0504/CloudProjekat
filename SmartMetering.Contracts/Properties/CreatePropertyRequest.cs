namespace SmartMetering.Contracts.Properties;

public record CreatePropertyRequest(
    string Name,
    string City,
    string Address,
    string? Description);

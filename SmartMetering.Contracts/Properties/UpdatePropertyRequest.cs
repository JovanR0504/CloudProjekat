namespace SmartMetering.Contracts.Properties;

public record UpdatePropertyRequest(
    string Name,
    string City,
    string Address,
    string? Description);

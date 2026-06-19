namespace SmartMetering.Contracts.Auth;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role);

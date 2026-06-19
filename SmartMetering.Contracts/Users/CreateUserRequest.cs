namespace SmartMetering.Contracts.Users;

public record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role);

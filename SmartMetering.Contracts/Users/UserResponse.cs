namespace SmartMetering.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role,
    bool IsActive,
    DateTime CreatedAtUtc);

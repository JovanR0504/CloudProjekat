using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class User : AggregateRoot
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public static Result<User> Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string phoneNumber,
        UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure(Error.Validation("User.EmptyEmail", "Email is required."));

        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure(Error.Validation("User.EmptyFirstName", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure(Error.Validation("User.EmptyLastName", "Last name is required."));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<User>.Failure(Error.Validation("User.EmptyPhoneNumber", "Phone number is required."));

        var user = new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Role = role,
            IsActive = false
        };

        return Result<User>.Success(user);
    }

    public void Update(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

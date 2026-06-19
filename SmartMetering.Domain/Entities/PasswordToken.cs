using SmartMetering.Domain.Common;
using SmartMetering.Domain.Enums;

namespace SmartMetering.Domain.Entities;

public class PasswordToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public TokenPurpose Purpose { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? UsedAtUtc { get; private set; }

    private PasswordToken() { }

    public static Result<PasswordToken> Create(
        Guid userId,
        string tokenHash,
        TokenPurpose purpose,
        DateTime expiresAtUtc)
    {
        if (userId == Guid.Empty)
            return Result<PasswordToken>.Failure(Error.Validation("PasswordToken.EmptyUserId", "User ID is required."));

        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result<PasswordToken>.Failure(Error.Validation("PasswordToken.EmptyTokenHash", "Token hash is required."));

        if (expiresAtUtc <= DateTime.UtcNow)
            return Result<PasswordToken>.Failure(Error.Validation("PasswordToken.InvalidExpiry", "Expiry must be in the future."));

        var token = new PasswordToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            Purpose = purpose,
            ExpiresAtUtc = expiresAtUtc
        };

        return Result<PasswordToken>.Success(token);
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAtUtc;
    public bool IsUsed() => UsedAtUtc.HasValue;

    public void MarkUsed()
    {
        UsedAtUtc = DateTime.UtcNow;
    }
}

namespace SmartMetering.Contracts.Auth;

public record LoginResponse(string Token, DateTime ExpiresAtUtc, string Role);

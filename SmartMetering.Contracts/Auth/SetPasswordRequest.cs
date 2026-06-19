namespace SmartMetering.Contracts.Auth;

public record SetPasswordRequest(string Token, string Password, string ConfirmPassword);

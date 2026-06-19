using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SmartMetering.Application.Interfaces;

namespace SmartMetering.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: HashSize);

        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

        return Convert.ToBase64String(result);
    }

    public bool Verify(string password, string storedHash)
    {
        var decoded = Convert.FromBase64String(storedHash);

        if (decoded.Length != SaltSize + HashSize)
            return false;

        var salt = new byte[SaltSize];
        Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: HashSize);

        for (int i = 0; i < HashSize; i++)
        {
            if (decoded[SaltSize + i] != hash[i])
                return false;
        }

        return true;
    }
}

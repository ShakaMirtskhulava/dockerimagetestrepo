using System.Security.Cryptography;
using GHotel.Application.Utilities;

namespace GHotel.Infrastructure.Utilities;

public class PasswordHasherUtility : IPasswordHasherUtility
{
    private const int SaltSize = 16; // Salt size in bytes
    private const int HashSize = 20; // Hash size in bytes
    private const int Iterations = 100000; // Number of iterations

    public string GenerateHash(string password)
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        var savedPasswordHash = Convert.ToBase64String(hashBytes);

        return savedPasswordHash;
    }

    public bool VerifyHash(string passwordHash, string givenPassword)
    {
        var hashBytes = Convert.FromBase64String(passwordHash);
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var pbkdf2 = new Rfc2898DeriveBytes(givenPassword, salt, Iterations);
        var computedHash = pbkdf2.GetBytes(HashSize);

        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != computedHash[i])
                return false;
        }

        return true;
    }
}

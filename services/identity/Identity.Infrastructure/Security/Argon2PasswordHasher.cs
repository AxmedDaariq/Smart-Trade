using Identity.Application.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Identity.Infrastructure.Security;

public class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 3;
    private const int MemorySize = 65536; // 64 MB
    private const int Parallelism = 4;
    private const string Argon2Prefix = "$argon2id$";

    public string HashPassword(string password)
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            Iterations = Iterations,
            MemorySize = MemorySize
        };

        var hash = argon2.GetBytes(HashSize);

        // Store as: $argon2id$v=19$m=65536,t=3,p=4$salt$hash
        return $"{Argon2Prefix}m={MemorySize},t={Iterations},p={Parallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash)) return false;

        // 1. Handle Argon2id Verification
        if (hash.StartsWith(Argon2Prefix, StringComparison.Ordinal))
        {
            var parts = hash.Split('$');
            if (parts.Length != 4) return false;

            try
            {
                var paramsPart = parts[2];
                var salt = Convert.FromBase64String(parts[3]);
                var expectedHash = Convert.FromBase64String(parts[4]);

                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    Salt = salt,
                    DegreeOfParallelism = Parallelism,
                    Iterations = Iterations,
                    MemorySize = MemorySize
                };

                var actualHash = argon2.GetBytes(HashSize);
                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch
            {
                return false;
            }
        }

        // 2. Backward Compatibility: Legacy PBKDF2 Verification
        return VerifyLegacyPbkdf2(password, hash);
    }

    private bool VerifyLegacyPbkdf2(string password, string hash)
    {
        // Handle ASP.NET Core Identity format (starts with AQAAAA)
        if (hash.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            var legacyHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
            var result = legacyHasher.VerifyHashedPassword(new object(), hash, password);
            return result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
        }

        // Handle the older custom PBKDF2 format (salt.hash)
        var parts = hash.Split('.', 2);
        if (parts.Length != 2) return false;
        try
        {
            var salt = Convert.FromBase64String(parts[0]);
            var expected = Convert.FromBase64String(parts[1]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch
        {
            return false;
        }
    }
}

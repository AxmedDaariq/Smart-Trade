using Identity.Infrastructure.Security;
using System.Security.Cryptography;
using Xunit;

namespace Identity.UnitTests;

public sealed class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_VerifiesCorrectPasswordAndRejectsWrongPassword()
    {
        var hash = _hasher.HashPassword("correct horse battery staple");

        Assert.StartsWith("pbkdf2-sha256$600000$", hash);
        Assert.True(_hasher.VerifyPassword("correct horse battery staple", hash));
        Assert.False(_hasher.VerifyPassword("wrong password", hash));
    }

    [Fact]
    public void VerifyPassword_AcceptsLegacyPbkdf2Hash()
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var derived = Rfc2898DeriveBytes.Pbkdf2("legacy-password", salt, 100_000, HashAlgorithmName.SHA256, 32);
        var legacyHash = $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(derived)}";

        Assert.True(_hasher.VerifyPassword("legacy-password", legacyHash));
        Assert.False(_hasher.VerifyPassword("not-the-password", legacyHash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("unknown$format")]
    [InlineData("pbkdf2-sha256$999999999$invalid$invalid")]
    public void VerifyPassword_RejectsMalformedHashes(string hash) =>
        Assert.False(_hasher.VerifyPassword("some-password", hash));
}

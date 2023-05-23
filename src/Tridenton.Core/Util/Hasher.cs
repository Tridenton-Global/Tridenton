using Microsoft.AspNetCore.Identity;

namespace Tridenton.Core.Util;

public readonly struct Hasher
{
    private static IPasswordHasher<object> _hasher => new PasswordHasher<object>();

    public static string Hash(string value) => _hasher.HashPassword(new(), value);

    public static bool VerifyHash(string hash, string value) => _hasher.VerifyHashedPassword(new(), hash, value) == PasswordVerificationResult.Success;
}
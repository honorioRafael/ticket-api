using Microsoft.AspNetCore.Identity;

namespace SharedKernel.Security;

public static class PasswordHasher
{
    private static readonly PasswordHasher<object> _hasher = new();

    public static string HashPassword(string password)
    {
        return _hasher.HashPassword(new object(), password);
    }

    public static bool VerifyPassword(string hashedPassword, string password)
    {
        var result = _hasher.VerifyHashedPassword(new object(), hashedPassword, password);
        return result != PasswordVerificationResult.Failed;
    }
}

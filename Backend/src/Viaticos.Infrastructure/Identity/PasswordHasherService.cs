using Microsoft.AspNetCore.Identity;
using Viaticos.Application.Common.Interfaces;

namespace Viaticos.Infrastructure.Identity;

public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<string> _hasher = new();

    public string HashPassword(string password) =>
        _hasher.HashPassword("user", password);

    public bool VerifyPassword(string password, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        return _hasher.VerifyHashedPassword("user", passwordHash, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}

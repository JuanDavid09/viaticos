namespace Viaticos.Application.Auth;

public static class AuthClaimTypes
{
    public const string MustChangePassword = "must_change_password";
}

public static class PasswordRules
{
    public const int MinLength = 8;

    public static bool IsStrongEnough(string password) =>
        !string.IsNullOrWhiteSpace(password)
        && password.Length >= MinLength
        && password.Any(char.IsUpper)
        && password.Any(char.IsLower)
        && password.Any(char.IsDigit);
}

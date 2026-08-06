namespace Viaticos.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ViaticosApi";
    public string Audience { get; set; } = "ViaticosClient";
    public int ExpirationMinutes { get; set; } = 480;
}

public static class AuthRoles
{
    public const string Empleado = "EMPLEADO";
    public const string JefeAprobador = "JEFE_APROBADOR";
    public const string Nomina = "NOMINA";
    public const string Admin = "ADMIN";
}

public static class AuthPolicies
{
    public const string Empleado = "Empleado";
    public const string Jefe = "Jefe";
    public const string Nomina = "Nomina";
    public const string Admin = "Admin";
}

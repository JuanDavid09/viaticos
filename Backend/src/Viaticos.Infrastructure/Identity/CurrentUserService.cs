using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Viaticos.Application.Common.Interfaces;

namespace Viaticos.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var idValue =
                User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(idValue, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
    }

    public string Email =>
        User?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? User?.FindFirstValue(ClaimTypes.Email)
        ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

    public bool IsInRole(string role)
    {
        if (User?.Identity?.IsAuthenticated != true)
            return false;

        var normalized = NormalizeRole(role);

        if (User.IsInRole(normalized))
            return true;

        return User.Claims.Any(claim =>
            IsRoleClaimType(claim.Type)
            && string.Equals(claim.Value, normalized, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsRoleClaimType(string claimType) =>
        string.Equals(claimType, JwtTokenService.RoleClaim, StringComparison.OrdinalIgnoreCase)
        || string.Equals(claimType, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase);

    private static string NormalizeRole(string role) => role.ToUpperInvariant() switch
    {
        "ADMIN" or "Admin" => AuthRoles.Admin,
        "EMPLEADO" or "Empleado" => AuthRoles.Empleado,
        "JEFE" or "JEFE_APROBADOR" or "JefeAprobador" => AuthRoles.JefeAprobador,
        "NOMINA" or "Nomina" => AuthRoles.Nomina,
        _ => role.ToUpperInvariant()
    };
}

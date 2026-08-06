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
            var idValue = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(idValue, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
    }

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)
        ?? throw new UnauthorizedAccessException("Usuario no autenticado.");

    public bool IsInRole(string role)
    {
        if (User?.Identity?.IsAuthenticated != true)
            return false;

        return User.IsInRole(NormalizeRole(role));
    }

    private static string NormalizeRole(string role) => role.ToUpperInvariant() switch
    {
        "ADMIN" or "Admin" => AuthRoles.Admin,
        "EMPLEADO" or "Empleado" => AuthRoles.Empleado,
        "JEFE" or "JEFE_APROBADOR" or "JefeAprobador" => AuthRoles.JefeAprobador,
        "NOMINA" or "Nomina" => AuthRoles.Nomina,
        _ => role.ToUpperInvariant()
    };
}

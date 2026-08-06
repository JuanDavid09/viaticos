using Viaticos.Application.Common.Interfaces;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Infrastructure.Identity;

public class DevCurrentUserService : ICurrentUserService
{
    public Guid UserId =>
        CurrentEmpleado?.Id
        ?? throw new InvalidOperationException("Usuario no autenticado. Use el header X-Dev-User-Email.");

    public string Email =>
        CurrentEmpleado?.Email
        ?? throw new InvalidOperationException("Usuario no autenticado. Use el header X-Dev-User-Email.");

    public Empleado? CurrentEmpleado { get; set; }

    public bool IsInRole(string role)
    {
        if (CurrentEmpleado is null) return false;

        return role.ToUpperInvariant() switch
        {
            "EMPLEADO" => CurrentEmpleado.Rol == Rol.Empleado,
            "JEFE_APROBADOR" => CurrentEmpleado.Rol == Rol.JefeAprobador,
            "NOMINA" => CurrentEmpleado.Rol == Rol.Nomina,
            "ADMIN" => CurrentEmpleado.Rol == Rol.Admin,
            _ => false
        };
    }
}

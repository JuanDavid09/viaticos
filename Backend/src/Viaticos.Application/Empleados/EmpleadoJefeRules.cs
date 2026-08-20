using Viaticos.Application.Common.Models;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Application.Empleados;

internal static class EmpleadoJefeRules
{
    public static Result ValidateJefeAssignment(
        Rol empleadoRol,
        Guid empleadoId,
        Guid? jefeId,
        Empleado? jefe)
    {
        if (empleadoRol == Rol.Empleado)
        {
            if (!jefeId.HasValue)
            {
                return Result.Failure(
                    "VALIDATION_ERROR",
                    "Los empleados deben tener un jefe aprobador asignado (campo jefe_id).");
            }
        }
        else if (jefeId.HasValue)
        {
            return Result.Failure(
                "VALIDATION_ERROR",
                "Solo los usuarios con rol Empleado pueden tener jefe asignado.");
        }

        if (!jefeId.HasValue)
            return Result.Success();

        if (jefeId.Value == empleadoId)
        {
            return Result.Failure(
                "VALIDATION_ERROR",
                "Un usuario no puede ser su propio jefe.");
        }

        if (jefe is null)
        {
            return Result.Failure(
                "VALIDATION_ERROR",
                "El jefe indicado no existe o está inactivo.");
        }

        if (jefe.Rol is not (Rol.JefeAprobador or Rol.Admin))
        {
            return Result.Failure(
                "VALIDATION_ERROR",
                "El jefe debe tener rol Jefe aprobador o Administrador.");
        }

        return Result.Success();
    }

    public static Guid? ResolveJefeId(Rol empleadoRol, Guid? jefeId) =>
        empleadoRol == Rol.Empleado ? jefeId : null;
}

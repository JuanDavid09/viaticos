using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Application.Empleados;

public record EmpleadoDto(
    Guid Id,
    string CodigoEmpleado,
    string Email,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string? Departamento,
    string Rol,
    Guid? JefeId,
    bool Activo,
    bool MustChangePassword);

public static class EmpleadoMapper
{
    public static EmpleadoDto ToDto(Empleado empleado) => new(
        empleado.Id,
        empleado.CodigoEmpleado,
        empleado.Email,
        empleado.Nombre,
        empleado.Apellido,
        empleado.NombreCompleto,
        empleado.Departamento,
        MapRol(empleado.Rol),
        empleado.JefeId,
        empleado.Activo,
        empleado.MustChangePassword);

    public static Rol ParseRol(string rol) => rol.Trim().ToUpperInvariant() switch
    {
        "EMPLEADO" => Rol.Empleado,
        "JEFE_APROBADOR" or "JEFE" => Rol.JefeAprobador,
        "NOMINA" => Rol.Nomina,
        "ADMIN" => Rol.Admin,
        _ => throw new ArgumentException($"Rol no soportado: {rol}")
    };

    public static string MapRol(Rol rol) => rol switch
    {
        Rol.Empleado => "EMPLEADO",
        Rol.JefeAprobador => "JEFE_APROBADOR",
        Rol.Nomina => "NOMINA",
        Rol.Admin => "ADMIN",
        _ => "EMPLEADO"
    };
}

namespace Viaticos.Domain.Core.Entities;

using Viaticos.Domain.Core.Enums;

public class Empleado : Entity
{
    public string CodigoEmpleado { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public string? Departamento { get; private set; }
    public Rol Rol { get; private set; }
    public Guid? JefeId { get; private set; }
    public bool Activo { get; private set; }

    private Empleado() { }

    public string NombreCompleto => $"{Nombre} {Apellido}";
}

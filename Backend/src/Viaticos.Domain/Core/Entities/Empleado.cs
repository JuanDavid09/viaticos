namespace Viaticos.Domain.Core.Entities;

using Viaticos.Domain.Common;
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
    public string? PasswordHash { get; private set; }
    public bool MustChangePassword { get; private set; }
    public bool Activo { get; private set; }

    private Empleado() { }

    public string NombreCompleto => $"{Nombre} {Apellido}";

    public static Empleado Crear(
        string codigoEmpleado,
        string email,
        string nombre,
        string apellido,
        Rol rol,
        string passwordHash,
        bool mustChangePassword,
        string? departamento = null,
        Guid? jefeId = null)
    {
        if (string.IsNullOrWhiteSpace(codigoEmpleado))
            throw new DomainException("CODIGO_REQUERIDO", "El código de empleado es obligatorio.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("EMAIL_REQUERIDO", "El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            throw new DomainException("NOMBRE_REQUERIDO", "Nombre y apellido son obligatorios.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PASSWORD_REQUERIDA", "La contraseña inicial es obligatoria.");

        return new Empleado
        {
            Id = Guid.NewGuid(),
            CodigoEmpleado = codigoEmpleado.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Departamento = departamento?.Trim(),
            Rol = rol,
            JefeId = jefeId,
            PasswordHash = passwordHash,
            MustChangePassword = mustChangePassword,
            Activo = true,
        };
    }

    public void ActualizarPerfil(
        string nombre,
        string apellido,
        Rol rol,
        string? departamento,
        Guid? jefeId)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            throw new DomainException("NOMBRE_REQUERIDO", "Nombre y apellido son obligatorios.");

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Rol = rol;
        Departamento = departamento?.Trim();
        JefeId = jefeId;
    }

    public void EstablecerPassword(string passwordHash, bool mustChangePassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PASSWORD_REQUERIDA", "La contraseña es obligatoria.");

        PasswordHash = passwordHash;
        MustChangePassword = mustChangePassword;
    }

    public void CompletarCambioPassword(string passwordHash)
    {
        EstablecerPassword(passwordHash, mustChangePassword: false);
    }

    public void Desactivar()
    {
        Activo = false;
    }

    public void Activar()
    {
        Activo = true;
    }
}

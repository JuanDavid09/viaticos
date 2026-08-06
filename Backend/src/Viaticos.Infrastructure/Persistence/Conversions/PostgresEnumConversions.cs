using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viaticos.Domain.Core.Enums;
using Viaticos.Domain.Documentos.Enums;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Infrastructure.Persistence.Conversions;

internal static class PostgresEnumConversions
{
    public static PropertyBuilder<Rol> HasPostgresEnum(this PropertyBuilder<Rol> property) =>
        property.HasColumnType("core.rol_enum")
            .HasConversion(
                v => MapRol(v),
                v => ParseRol(v));

    public static PropertyBuilder<EstadoLegalizacion> HasPostgresEnum(this PropertyBuilder<EstadoLegalizacion> property) =>
        property.HasColumnType("viaticos.estado_legalizacion_enum")
            .HasConversion(
                v => MapEstadoLegalizacion(v),
                v => ParseEstadoLegalizacion(v));

    public static PropertyBuilder<EstadoLegalizacion?> HasPostgresEnum(this PropertyBuilder<EstadoLegalizacion?> property) =>
        property.HasColumnType("viaticos.estado_legalizacion_enum")
            .HasConversion(
                v => v.HasValue ? MapEstadoLegalizacion(v.Value) : null,
                v => v == null ? null : ParseEstadoLegalizacion(v));

    public static PropertyBuilder<EstadoOcr> HasPostgresEnum(this PropertyBuilder<EstadoOcr> property) =>
        property.HasColumnType("docs.estado_ocr_enum")
            .HasConversion(
                v => MapEstadoOcr(v),
                v => ParseEstadoOcr(v));

    private static string MapRol(Rol rol) => rol switch
    {
        Rol.Empleado => "EMPLEADO",
        Rol.JefeAprobador => "JEFE_APROBADOR",
        Rol.Nomina => "NOMINA",
        Rol.Admin => "ADMIN",
        _ => throw new ArgumentOutOfRangeException(nameof(rol))
    };

    private static Rol ParseRol(string value) => value switch
    {
        "EMPLEADO" => Rol.Empleado,
        "JEFE_APROBADOR" => Rol.JefeAprobador,
        "NOMINA" => Rol.Nomina,
        "ADMIN" => Rol.Admin,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static string MapEstadoLegalizacion(EstadoLegalizacion estado) => estado switch
    {
        EstadoLegalizacion.Borrador => "BORRADOR",
        EstadoLegalizacion.PendienteValidacion => "PENDIENTE_VALIDACION",
        EstadoLegalizacion.PendienteAprobacion => "PENDIENTE_APROBACION",
        EstadoLegalizacion.Aprobada => "APROBADA",
        EstadoLegalizacion.Rechazada => "RECHAZADA",
        EstadoLegalizacion.PendienteNomina => "PENDIENTE_NOMINA",
        EstadoLegalizacion.Cerrada => "CERRADA",
        _ => throw new ArgumentOutOfRangeException(nameof(estado))
    };

    private static EstadoLegalizacion ParseEstadoLegalizacion(string value) => value switch
    {
        "BORRADOR" => EstadoLegalizacion.Borrador,
        "PENDIENTE_VALIDACION" => EstadoLegalizacion.PendienteValidacion,
        "PENDIENTE_APROBACION" => EstadoLegalizacion.PendienteAprobacion,
        "APROBADA" => EstadoLegalizacion.Aprobada,
        "RECHAZADA" => EstadoLegalizacion.Rechazada,
        "PENDIENTE_NOMINA" => EstadoLegalizacion.PendienteNomina,
        "CERRADA" => EstadoLegalizacion.Cerrada,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static string MapEstadoOcr(EstadoOcr estado) => estado switch
    {
        EstadoOcr.Pendiente => "PENDIENTE",
        EstadoOcr.Procesando => "PROCESANDO",
        EstadoOcr.Completado => "COMPLETADO",
        EstadoOcr.Error => "ERROR",
        EstadoOcr.ValidadoUsuario => "VALIDADO_USUARIO",
        _ => throw new ArgumentOutOfRangeException(nameof(estado))
    };

    private static EstadoOcr ParseEstadoOcr(string value) => value switch
    {
        "PENDIENTE" => EstadoOcr.Pendiente,
        "PROCESANDO" => EstadoOcr.Procesando,
        "COMPLETADO" => EstadoOcr.Completado,
        "ERROR" => EstadoOcr.Error,
        "VALIDADO_USUARIO" => EstadoOcr.ValidadoUsuario,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}

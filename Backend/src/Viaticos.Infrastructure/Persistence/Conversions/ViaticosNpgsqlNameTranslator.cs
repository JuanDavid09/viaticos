using Npgsql;
using Viaticos.Domain.Core.Enums;
using Viaticos.Domain.Documentos.Enums;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Infrastructure.Persistence.Conversions;

internal sealed class ViaticosNpgsqlNameTranslator : INpgsqlNameTranslator
{
    public string TranslateTypeName(string clrName) => clrName;

    public string TranslateMemberName(string clrName) => clrName switch
    {
        nameof(Rol.Empleado) => "EMPLEADO",
        nameof(Rol.JefeAprobador) => "JEFE_APROBADOR",
        nameof(Rol.Nomina) => "NOMINA",
        nameof(Rol.Admin) => "ADMIN",

        nameof(EstadoLegalizacion.Borrador) => "BORRADOR",
        nameof(EstadoLegalizacion.PendienteValidacion) => "PENDIENTE_VALIDACION",
        nameof(EstadoLegalizacion.PendienteAprobacion) => "PENDIENTE_APROBACION",
        nameof(EstadoLegalizacion.Aprobada) => "APROBADA",
        nameof(EstadoLegalizacion.Rechazada) => "RECHAZADA",
        nameof(EstadoLegalizacion.PendienteNomina) => "PENDIENTE_NOMINA",
        nameof(EstadoLegalizacion.Cerrada) => "CERRADA",

        nameof(EstadoOcr.Pendiente) => "PENDIENTE",
        nameof(EstadoOcr.Procesando) => "PROCESANDO",
        nameof(EstadoOcr.Completado) => "COMPLETADO",
        nameof(EstadoOcr.Error) => "ERROR",
        nameof(EstadoOcr.ValidadoUsuario) => "VALIDADO_USUARIO",

        _ => clrName
    };
}

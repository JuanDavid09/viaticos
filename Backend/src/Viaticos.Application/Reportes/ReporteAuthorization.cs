using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;

namespace Viaticos.Application.Reportes;

public static class ReporteAuthorization
{
    public static Result EnsureCanAccess(ICurrentUserService currentUser, ReporteTipo tipo)
    {
        if (currentUser.IsInRole("Admin"))
            return Result.Success();

        var allowed = tipo switch
        {
            ReporteTipo.PendientesAprobacion =>
                currentUser.IsInRole("JefeAprobador"),
            ReporteTipo.PendientesNomina =>
                currentUser.IsInRole("Nomina"),
            ReporteTipo.VolumenMensual or ReporteTipo.TiemposPorEstado =>
                currentUser.IsInRole("Nomina") || currentUser.IsInRole("JefeAprobador"),
            _ =>
                currentUser.IsInRole("JefeAprobador") || currentUser.IsInRole("Nomina")
        };

        return allowed
            ? Result.Success()
            : Result.Failure("FORBIDDEN", "No tiene permiso para consultar este reporte.");
    }

    public static ReporteFiltros ApplyScope(ICurrentUserService currentUser, ReporteFiltros filtros)
    {
        if (currentUser.IsInRole("Admin") || currentUser.IsInRole("Nomina"))
            return filtros;

        if (currentUser.IsInRole("JefeAprobador"))
        {
            return filtros with
            {
                JefeId = currentUser.UserId,
                EmpleadoId = filtros.EmpleadoId
            };
        }

        return filtros with { EmpleadoId = currentUser.UserId };
    }
}

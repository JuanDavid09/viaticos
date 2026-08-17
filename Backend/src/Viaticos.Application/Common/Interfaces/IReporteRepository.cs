using Viaticos.Application.Reportes;

namespace Viaticos.Application.Common.Interfaces;

public interface IReporteRepository
{
    Task<IReadOnlyList<ResumenPorEstadoDto>> GetResumenPorEstadoAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LegalizacionDetalleReporteDto>> GetLegalizacionesDetalleAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GastoPorCategoriaDto>> GetGastosPorCategoriaAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GastoDetalleReporteDto>> GetGastosDetalleAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResumenFinancieroEmpleadoDto>> GetResumenFinancieroEmpleadoAsync(
        ReporteFiltros filtros,
        bool soloCerradas,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PendienteAprobacionReporteDto>> GetPendientesAprobacionAsync(
        Guid? jefeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PendienteNominaReporteDto>> GetPendientesNominaAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LegalizacionCerradaReporteDto>> GetLegalizacionesCerradasAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GastoSinSoporteDto>> GetGastosSinSoporteAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HistorialAuditoriaDto>> GetHistorialAuditoriaAsync(
        ReporteFiltros filtros,
        Guid? legalizacionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VolumenMensualDto>> GetVolumenMensualAsync(
        ReporteFiltros filtros,
        int? anio,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TiempoPorEstadoDto>> GetTiemposPorEstadoAsync(
        ReporteFiltros filtros,
        CancellationToken cancellationToken = default);
}

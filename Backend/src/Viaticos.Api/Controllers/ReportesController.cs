using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Reportes.Queries;
using Viaticos.Domain.Legalizaciones.Enums;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = Infrastructure.Identity.AuthPolicies.Empleado)]
[Route("api/[controller]")]
public class ReportesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("resumen-por-estado")]
    public async Task<IActionResult> ResumenPorEstado(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetResumenPorEstadoReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("legalizaciones-detalle")]
    public async Task<IActionResult> LegalizacionesDetalle(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLegalizacionesDetalleReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("gastos-por-categoria")]
    public async Task<IActionResult> GastosPorCategoria(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetGastosPorCategoriaReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("gastos-detalle")]
    public async Task<IActionResult> GastosDetalle(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetGastosDetalleReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("resumen-financiero-empleado")]
    public async Task<IActionResult> ResumenFinancieroEmpleado(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetResumenFinancieroEmpleadoReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("pendientes-aprobacion")]
    public async Task<IActionResult> PendientesAprobacion(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPendientesAprobacionReporteQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("pendientes-nomina")]
    public async Task<IActionResult> PendientesNomina(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPendientesNominaReporteQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("legalizaciones-cerradas")]
    public async Task<IActionResult> LegalizacionesCerradas(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLegalizacionesCerradasReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("gastos-sin-soporte")]
    public async Task<IActionResult> GastosSinSoporte(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetGastosSinSoporteReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("historial-auditoria")]
    public async Task<IActionResult> HistorialAuditoria(
        [FromQuery] ReporteFiltrosQuery filtros,
        [FromQuery] Guid? legalizacionId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetHistorialAuditoriaReporteQuery(filtros.ToRequest(), legalizacionId),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("volumen-mensual")]
    public async Task<IActionResult> VolumenMensual(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetVolumenMensualReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }

    [HttpGet("tiempos-por-estado")]
    public async Task<IActionResult> TiemposPorEstado(
        [FromQuery] ReporteFiltrosQuery filtros,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTiemposPorEstadoReporteQuery(filtros.ToRequest()),
            cancellationToken);
        return FromResult(result);
    }
}

public record ReporteFiltrosQuery(
    DateOnly? Desde = null,
    DateOnly? Hasta = null,
    Guid? EmpleadoId = null,
    Guid? JefeId = null,
    string? Departamento = null,
    EstadoLegalizacion? Estado = null,
    int? Anio = null,
    bool SoloCerradas = true)
{
    public ReporteFiltrosRequest ToRequest() =>
        new(Desde, Hasta, EmpleadoId, JefeId, Departamento, Estado, Anio, SoloCerradas);
}

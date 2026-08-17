using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Legalizaciones.Commands;
using Viaticos.Application.Legalizaciones.Queries;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = Infrastructure.Identity.AuthPolicies.Empleado)]
[Route("api/[controller]")]
public class LegalizacionesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public LegalizacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarMisLegalizacionesQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("calendario")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Jefe)]
    public async Task<IActionResult> ListarCalendario(
        [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarCalendarioLegalizacionesQuery(desde, hasta), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerLegalizacionQuery(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearLegalizacionRequest request, CancellationToken cancellationToken)
    {
        var command = new CrearLegalizacionCommand(
            request.Motivo,
            request.FechaInicio,
            request.FechaFin,
            request.MonedaId,
            request.MontoAnticipo,
            request.Destino);

        var result = await _mediator.Send(command, cancellationToken);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarLegalizacionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ActualizarLegalizacionCommand(
            id,
            request.Motivo,
            request.FechaInicio,
            request.FechaFin,
            request.MonedaId,
            request.MontoAnticipo,
            request.Destino);

        var result = await _mediator.Send(command, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/gastos")]
    public async Task<IActionResult> AgregarGasto(
        Guid id,
        [FromBody] AgregarGastoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AgregarGastoCommand(
            id,
            request.CategoriaGastoId,
            request.FechaGasto,
            request.Descripcion,
            request.Monto,
            request.Proveedor,
            request.NumeroDocumento);

        var result = await _mediator.Send(command, cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/historial")]
    public async Task<IActionResult> ObtenerHistorial(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerHistorialQuery(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/enviar-validacion")]
    public async Task<IActionResult> EnviarValidacion(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EnviarValidacionCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/enviar-aprobacion")]
    public async Task<IActionResult> EnviarAprobacion(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EnviarAprobacionCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/aprobar")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Jefe)]
    public async Task<IActionResult> Aprobar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AprobarLegalizacionCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/rechazar")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Jefe)]
    public async Task<IActionResult> Rechazar(
        Guid id,
        [FromBody] RechazarLegalizacionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RechazarLegalizacionCommand(id, request.Comentario), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/reabrir")]
    public async Task<IActionResult> Reabrir(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReabrirLegalizacionCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/enviar-nomina")]
    public async Task<IActionResult> EnviarNomina(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EnviarNominaCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/cerrar")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Nomina)]
    public async Task<IActionResult> Cerrar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CerrarLegalizacionCommand(id), cancellationToken);
        return FromResult(result);
    }
}

public record CrearLegalizacionRequest(
    string Motivo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string? Destino);

public record ActualizarLegalizacionRequest(
    string Motivo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string? Destino);

public record AgregarGastoRequest(
    Guid CategoriaGastoId,
    DateOnly FechaGasto,
    string Descripcion,
    decimal Monto,
    string? Proveedor,
    string? NumeroDocumento);

public record RechazarLegalizacionRequest(string Comentario);

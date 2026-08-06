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

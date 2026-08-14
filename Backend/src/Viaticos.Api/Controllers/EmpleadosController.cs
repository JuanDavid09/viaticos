using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Empleados.Commands;
using Viaticos.Application.Empleados.Queries;
using Viaticos.Infrastructure.Identity;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = AuthPolicies.Admin)]
[Route("api/[controller]")]
public class EmpleadosController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public EmpleadosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListarEmpleadosQuery(includeInactive), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerEmpleadoQuery(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEmpleadoRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CrearEmpleadoCommand(
                request.CodigoEmpleado,
                request.Email,
                request.Nombre,
                request.Apellido,
                request.Rol,
                request.PasswordTemporal,
                request.Departamento,
                request.JefeId),
            cancellationToken);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid id,
        [FromBody] ActualizarEmpleadoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ActualizarEmpleadoCommand(
                id,
                request.Nombre,
                request.Apellido,
                request.Rol,
                request.Departamento,
                request.JefeId,
                request.Activo),
            cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/restablecer-password")]
    public async Task<IActionResult> RestablecerPassword(
        Guid id,
        [FromBody] RestablecerPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RestablecerPasswordEmpleadoCommand(id, request.PasswordTemporal),
            cancellationToken);
        return FromResult(result);
    }
}

public record CrearEmpleadoRequest(
    string CodigoEmpleado,
    string Email,
    string Nombre,
    string Apellido,
    string Rol,
    string PasswordTemporal,
    string? Departamento,
    Guid? JefeId);

public record ActualizarEmpleadoRequest(
    string Nombre,
    string Apellido,
    string Rol,
    string? Departamento,
    Guid? JefeId,
    bool Activo);

public record RestablecerPasswordRequest(string PasswordTemporal);

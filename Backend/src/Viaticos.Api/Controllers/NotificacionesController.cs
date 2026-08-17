using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Notificaciones.Commands;
using Viaticos.Application.Notificaciones.Queries;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = Infrastructure.Identity.AuthPolicies.Empleado)]
[Route("api/[controller]")]
public class NotificacionesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public NotificacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int limite = 20, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListarNotificacionesQuery(limite), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerResumenNotificacionesQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpPatch("{id:guid}/leida")]
    public async Task<IActionResult> MarcarLeida(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarcarNotificacionLeidaCommand(id), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("marcar-todas-leidas")]
    public async Task<IActionResult> MarcarTodasLeidas(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarcarTodasNotificacionesLeidasCommand(), cancellationToken);
        return FromResult(result);
    }
}

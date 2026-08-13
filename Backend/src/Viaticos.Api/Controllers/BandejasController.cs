using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Legalizaciones.Queries;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = Infrastructure.Identity.AuthPolicies.Empleado)]
[Route("api/[controller]")]
public class BandejasController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BandejasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("mis-legalizaciones")]
    public async Task<IActionResult> MisLegalizaciones(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarMisLegalizacionesQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("pendientes-aprobacion")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Jefe)]
    public async Task<IActionResult> PendientesAprobacion(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarPendientesAprobacionQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("pendientes-nomina")]
    [Authorize(Policy = Infrastructure.Identity.AuthPolicies.Nomina)]
    public async Task<IActionResult> PendientesNomina(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListarPendientesNominaQuery(), cancellationToken);
        return FromResult(result);
    }
}

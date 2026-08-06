using MediatR;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Catalogos.Queries;

namespace Viaticos.Api.Controllers;

[Route("api/[controller]")]
public class CatalogosController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public CatalogosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCatalogosQuery(), cancellationToken);
        return FromResult(result);
    }
}

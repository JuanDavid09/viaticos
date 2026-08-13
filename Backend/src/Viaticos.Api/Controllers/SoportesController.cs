using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viaticos.Application.Documentos.Commands;
using Viaticos.Application.Documentos.Queries;

namespace Viaticos.Api.Controllers;

[Authorize(Policy = Infrastructure.Identity.AuthPolicies.Empleado)]
[Route("api/[controller]")]
[RequestSizeLimit(10 * 1024 * 1024)]
public class SoportesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SoportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sube un soporte (factura/recibo) y lo vincula a un gasto.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Subir([FromForm] SubirSoporteFormRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { code = "ARCHIVO_REQUERIDO", message = "Debe adjuntar un archivo." });

        await using var stream = request.File.OpenReadStream();
        var command = new SubirSoporteCommand(
            request.LegalizacionId,
            request.GastoId,
            stream,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            request.EsPrincipal);

        var result = await _mediator.Send(command, cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{gastoSoporteId:guid}/ocr")]
    public async Task<IActionResult> ObtenerOcr(Guid gastoSoporteId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ObtenerOcrExtraccionQuery(gastoSoporteId), cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{gastoSoporteId:guid}/ocr/procesar")]
    public async Task<IActionResult> ProcesarOcr(Guid gastoSoporteId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ProcesarOcrCommand(gastoSoporteId), cancellationToken);
        return FromResult(result);
    }

    [HttpPut("{gastoSoporteId:guid}/ocr/campos")]
    public async Task<IActionResult> ValidarCampos(
        Guid gastoSoporteId,
        [FromBody] ValidarCamposOcrRequestBody request,
        CancellationToken cancellationToken)
    {
        var command = new ValidarCamposOcrCommand(
            gastoSoporteId,
            request.Campos.Select(c => new ValidarCampoOcrRequest(c.CampoId, c.ValorValidado)).ToList());

        var result = await _mediator.Send(command, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{gastoSoporteId:guid}/ocr/aplicar")]
    public async Task<IActionResult> AplicarOcr(Guid gastoSoporteId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AplicarOcrAGastoCommand(gastoSoporteId), cancellationToken);
        return FromResult(result);
    }
}

public class SubirSoporteFormRequest
{
    public Guid LegalizacionId { get; set; }
    public Guid GastoId { get; set; }
    public IFormFile? File { get; set; }
    public bool EsPrincipal { get; set; }
}

public record ValidarCamposOcrRequestBody(IReadOnlyList<ValidarCampoOcrBodyItem> Campos);

public record ValidarCampoOcrBodyItem(Guid CampoId, string ValorValidado);

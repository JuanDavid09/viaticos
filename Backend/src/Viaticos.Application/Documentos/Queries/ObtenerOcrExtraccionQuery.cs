using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Documentos.DTOs;

namespace Viaticos.Application.Documentos.Queries;

public record ObtenerOcrExtraccionQuery(Guid GastoSoporteId) : IRequest<Result<OcrExtraccionDto>>;

public class ObtenerOcrExtraccionQueryHandler : IRequestHandler<ObtenerOcrExtraccionQuery, Result<OcrExtraccionDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ICurrentUserService _currentUser;

    public ObtenerOcrExtraccionQueryHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ICurrentUserService currentUser)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<OcrExtraccionDto>> Handle(ObtenerOcrExtraccionQuery request, CancellationToken cancellationToken)
    {
        var soporte = await _documentoRepository.GetGastoSoporteByIdAsync(request.GastoSoporteId, cancellationToken);
        if (soporte is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Soporte no encontrado.");

        var legalizacion = await _legalizacionRepository.GetByGastoIdAsync(soporte.GastoId, cancellationToken);
        if (legalizacion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            return Result<OcrExtraccionDto>.Failure("FORBIDDEN", "No tiene permiso para ver este soporte.");

        var extraccion = await _documentoRepository.GetOcrExtraccionByGastoSoporteIdAsync(request.GastoSoporteId, cancellationToken);
        if (extraccion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Extracción OCR no encontrada.");

        return Result<OcrExtraccionDto>.Success(SoporteMapper.ToDto(extraccion));
    }
}

using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Documentos.DTOs;
using Viaticos.Application.Legalizaciones.Services;
using Viaticos.Domain.Common;
using Viaticos.Domain.Documentos.Entities;

namespace Viaticos.Application.Documentos.Commands;

public record SubirSoporteCommand(
    Guid LegalizacionId,
    Guid GastoId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize,
    bool EsPrincipal) : IRequest<Result<SubirSoporteResponseDto>>;

public class SubirSoporteCommandValidator : AbstractValidator<SubirSoporteCommand>
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "application/pdf"
    ];

    public SubirSoporteCommandValidator()
    {
        RuleFor(x => x.LegalizacionId).NotEmpty();
        RuleFor(x => x.GastoId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.FileSize).GreaterThan(0).LessThanOrEqualTo(10 * 1024 * 1024);
        RuleFor(x => x.ContentType)
            .Must(type => AllowedContentTypes.Contains(type.ToLowerInvariant()))
            .WithMessage("Tipo de archivo no permitido. Use JPG, PNG o PDF.");
    }
}

public class SubirSoporteCommandHandler : IRequestHandler<SubirSoporteCommand, Result<SubirSoporteResponseDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public SubirSoporteCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SubirSoporteResponseDto>> Handle(SubirSoporteCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<SubirSoporteResponseDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<SubirSoporteResponseDto>.Failure("FORBIDDEN", "No tiene permiso para modificar esta legalización.");

        if (legalizacion.Gastos.All(g => g.Id != request.GastoId))
            return Result<SubirSoporteResponseDto>.Failure("GASTO_NOT_FOUND", "Gasto no encontrado en la legalización.");

        try
        {
            var (bucket, objectKey) = await _fileStorageService.UploadAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken);

            var archivo = Archivo.Crear(bucket, objectKey, request.FileName, request.ContentType, request.FileSize, _currentUser.UserId);
            var soporte = Domain.Legalizaciones.Entities.GastoSoporte.Crear(request.GastoId, archivo.Id, _currentUser.UserId, request.EsPrincipal);
            var ocr = OcrExtraccion.Crear(archivo.Id, soporte.Id);

            await _documentoRepository.AddArchivoAsync(archivo, cancellationToken);
            await _documentoRepository.AddGastoSoporteAsync(soporte, cancellationToken);
            await _documentoRepository.AddOcrExtraccionAsync(ocr, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SubirSoporteResponseDto>.Success(new SubirSoporteResponseDto(
                soporte.Id,
                archivo.Id,
                ocr.Id,
                archivo.NombreOriginal));
        }
        catch (DomainException ex)
        {
            return Result<SubirSoporteResponseDto>.Failure(ex.Code, ex.Message);
        }
    }
}

public record ProcesarOcrCommand(Guid GastoSoporteId) : IRequest<Result<OcrExtraccionDto>>;

public class ProcesarOcrCommandHandler : IRequestHandler<ProcesarOcrCommand, Result<OcrExtraccionDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IOcrService _ocrService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ProcesarOcrCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        IFileStorageService fileStorageService,
        IOcrService ocrService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _fileStorageService = fileStorageService;
        _ocrService = ocrService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OcrExtraccionDto>> Handle(ProcesarOcrCommand request, CancellationToken cancellationToken)
    {
        var soporte = await _documentoRepository.GetGastoSoporteByIdAsync(request.GastoSoporteId, cancellationToken);
        if (soporte is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Soporte no encontrado.");

        var legalizacion = await _legalizacionRepository.GetByGastoIdAsync(soporte.GastoId, cancellationToken);
        if (legalizacion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<OcrExtraccionDto>.Failure("FORBIDDEN", "No tiene permiso para procesar este soporte.");

        var extraccion = await _documentoRepository.GetOcrExtraccionByGastoSoporteIdAsync(request.GastoSoporteId, cancellationToken);
        if (extraccion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Extracción OCR no encontrada.");

        var archivo = await _documentoRepository.GetArchivoByIdAsync(soporte.ArchivoId, cancellationToken);
        if (archivo is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Archivo no encontrado.");

        try
        {
            extraccion.MarcarProcesando();
            _documentoRepository.UpdateOcrExtraccion(extraccion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await using var stream = await _fileStorageService.DownloadAsync(archivo.Bucket, archivo.ObjectKey, cancellationToken);
            var ocrResult = await _ocrService.AnalyzeReceiptAsync(stream, cancellationToken);

            extraccion.Completar(
                ocrResult.RawJson,
                ocrResult.Fields.Select(f => (f.Key, (string?)f.Value)));

            _documentoRepository.UpdateOcrExtraccion(extraccion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OcrExtraccionDto>.Success(SoporteMapper.ToDto(extraccion));
        }
        catch (DomainException ex)
        {
            return Result<OcrExtraccionDto>.Failure(ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            extraccion.MarcarError(ex.Message);
            _documentoRepository.UpdateOcrExtraccion(extraccion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<OcrExtraccionDto>.Failure("OCR_ERROR", "Error al procesar OCR.");
        }
    }
}

public record ValidarCampoOcrRequest(Guid CampoId, string ValorValidado);

public record ValidarCamposOcrCommand(Guid GastoSoporteId, IReadOnlyList<ValidarCampoOcrRequest> Campos)
    : IRequest<Result<OcrExtraccionDto>>;

public class ValidarCamposOcrCommandValidator : AbstractValidator<ValidarCamposOcrCommand>
{
    public ValidarCamposOcrCommandValidator()
    {
        RuleFor(x => x.GastoSoporteId).NotEmpty();
        RuleFor(x => x.Campos).NotEmpty();
        RuleForEach(x => x.Campos).ChildRules(c =>
        {
            c.RuleFor(x => x.CampoId).NotEmpty();
            c.RuleFor(x => x.ValorValidado).NotEmpty();
        });
    }
}

public class ValidarCamposOcrCommandHandler : IRequestHandler<ValidarCamposOcrCommand, Result<OcrExtraccionDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ValidarCamposOcrCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OcrExtraccionDto>> Handle(ValidarCamposOcrCommand request, CancellationToken cancellationToken)
    {
        var soporte = await _documentoRepository.GetGastoSoporteByIdAsync(request.GastoSoporteId, cancellationToken);
        if (soporte is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Soporte no encontrado.");

        var legalizacion = await _legalizacionRepository.GetByGastoIdAsync(soporte.GastoId, cancellationToken);
        if (legalizacion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<OcrExtraccionDto>.Failure("FORBIDDEN", "No tiene permiso para validar este soporte.");

        var extraccion = await _documentoRepository.GetOcrExtraccionByGastoSoporteIdAsync(request.GastoSoporteId, cancellationToken);
        if (extraccion is null)
            return Result<OcrExtraccionDto>.Failure("NOT_FOUND", "Extracción OCR no encontrada.");

        try
        {
            extraccion.ValidarCampos(
                request.Campos.Select(c => (c.CampoId, c.ValorValidado)),
                _currentUser.UserId);

            _documentoRepository.UpdateOcrExtraccion(extraccion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<OcrExtraccionDto>.Success(SoporteMapper.ToDto(extraccion));
        }
        catch (DomainException ex)
        {
            return Result<OcrExtraccionDto>.Failure(ex.Code, ex.Message);
        }
    }
}

public record AplicarOcrAGastoCommand(Guid GastoSoporteId) : IRequest<Result<Legalizaciones.DTOs.LegalizacionDetalleDto>>;

public class AplicarOcrAGastoCommandHandler : IRequestHandler<AplicarOcrAGastoCommand, Result<Legalizaciones.DTOs.LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AplicarOcrAGastoCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Legalizaciones.DTOs.LegalizacionDetalleDto>> Handle(
        AplicarOcrAGastoCommand request,
        CancellationToken cancellationToken)
    {
        var soporte = await _documentoRepository.GetGastoSoporteByIdAsync(request.GastoSoporteId, cancellationToken);
        if (soporte is null)
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure("NOT_FOUND", "Soporte no encontrado.");

        var legalizacion = await _legalizacionRepository.GetByGastoIdAsync(soporte.GastoId, cancellationToken);
        if (legalizacion is null)
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure("FORBIDDEN", "No tiene permiso para modificar esta legalización.");

        var extraccion = await _documentoRepository.GetOcrExtraccionByGastoSoporteIdAsync(request.GastoSoporteId, cancellationToken);
        if (extraccion is null)
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure("NOT_FOUND", "Extracción OCR no encontrada.");

        if (extraccion.Estado is not Domain.Documentos.Enums.EstadoOcr.ValidadoUsuario and not Domain.Documentos.Enums.EstadoOcr.Completado)
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure("OCR_ESTADO_INVALIDO", "Debe validar los campos OCR antes de aplicarlos al gasto.");

        try
        {
            var valores = extraccion.ObtenerValoresValidados();
            var (proveedor, numeroDocumento, monto, fechaGasto) = OcrMappingService.MapToGastoFields(valores);

            legalizacion.AplicarOcrAGasto(soporte.GastoId, proveedor, numeroDocumento, monto, fechaGasto);
            _legalizacionRepository.Update(legalizacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _legalizacionRepository.GetByIdAsync(legalizacion.Id, cancellationToken);
            var soportes = await _documentoRepository.ListSoportesByGastoIdsAsync(
                updated!.Gastos.Select(g => g.Id),
                cancellationToken);

            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Success(
                await _detalleFactory.CreateAsync(updated!, soportes, cancellationToken));
        }
        catch (DomainException ex)
        {
            return Result<Legalizaciones.DTOs.LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

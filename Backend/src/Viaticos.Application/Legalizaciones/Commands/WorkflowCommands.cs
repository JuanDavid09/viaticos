using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Domain.Common;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.Commands;

public record EnviarValidacionCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public record EnviarAprobacionCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public record AprobarLegalizacionCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public record RechazarLegalizacionCommand(Guid LegalizacionId, string Comentario) : IRequest<Result<LegalizacionDetalleDto>>;

public record ReabrirLegalizacionCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public record EnviarNominaCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public record CerrarLegalizacionCommand(Guid LegalizacionId) : IRequest<Result<LegalizacionDetalleDto>>;

public class RechazarLegalizacionCommandValidator : AbstractValidator<RechazarLegalizacionCommand>
{
    public RechazarLegalizacionCommandValidator()
    {
        RuleFor(x => x.LegalizacionId).NotEmpty();
        RuleFor(x => x.Comentario).NotEmpty().MaximumLength(2000);
    }
}

internal static class WorkflowCommandHelper
{
    public static async Task<Result<LegalizacionDetalleDto>> ExecuteAsync(
        Guid legalizacionId,
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        IUnitOfWork unitOfWork,
        Action<Legalizacion> action,
        CancellationToken cancellationToken)
    {
        var legalizacion = await legalizacionRepository.GetByIdAsync(legalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        try
        {
            action(legalizacion);
            legalizacionRepository.Update(legalizacion);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await legalizacionRepository.GetByIdAsync(legalizacionId, cancellationToken);
            var soportes = await documentoRepository.ListSoportesByGastoIdsAsync(
                updated!.Gastos.Select(g => g.Id),
                cancellationToken);

            return Result<LegalizacionDetalleDto>.Success(LegalizacionMapper.ToDetalle(updated, soportes));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

public class EnviarValidacionCommandHandler : IRequestHandler<EnviarValidacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public EnviarValidacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(EnviarValidacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        var auth = _workflow.EnsureIsOwner(legalizacion, _currentUser.UserId);
        if (!auth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.EnviarValidacion(_currentUser.UserId),
            cancellationToken);
    }
}

public class EnviarAprobacionCommandHandler : IRequestHandler<EnviarAprobacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public EnviarAprobacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(EnviarAprobacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        var auth = _workflow.EnsureIsOwner(legalizacion, _currentUser.UserId);
        if (!auth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.EnviarAprobacion(_currentUser.UserId),
            cancellationToken);
    }
}

public class AprobarLegalizacionCommandHandler : IRequestHandler<AprobarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AprobarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(AprobarLegalizacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (!_currentUser.IsInRole("Admin"))
        {
            var auth = await _workflow.EnsureIsJefeDelEmpleadoAsync(legalizacion, _currentUser.UserId, cancellationToken);
            if (!auth.IsSuccess)
                return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);
        }

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.Aprobar(_currentUser.UserId),
            cancellationToken);
    }
}

public class RechazarLegalizacionCommandHandler : IRequestHandler<RechazarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RechazarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(RechazarLegalizacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (!_currentUser.IsInRole("Admin"))
        {
            var auth = await _workflow.EnsureIsJefeDelEmpleadoAsync(legalizacion, _currentUser.UserId, cancellationToken);
            if (!auth.IsSuccess)
                return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);
        }

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.Rechazar(_currentUser.UserId, request.Comentario),
            cancellationToken);
    }
}

public class ReabrirLegalizacionCommandHandler : IRequestHandler<ReabrirLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ReabrirLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(ReabrirLegalizacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        var auth = _workflow.EnsureIsOwner(legalizacion, _currentUser.UserId);
        if (!auth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.Reabrir(_currentUser.UserId),
            cancellationToken);
    }
}

public class EnviarNominaCommandHandler : IRequestHandler<EnviarNominaCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public EnviarNominaCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(EnviarNominaCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        var auth = _workflow.EnsureIsOwner(legalizacion, _currentUser.UserId);
        if (!auth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.EnviarNomina(_currentUser.UserId),
            cancellationToken);
    }
}

public class CerrarLegalizacionCommandHandler : IRequestHandler<CerrarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CerrarLegalizacionCommandHandler(
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

    public async Task<Result<LegalizacionDetalleDto>> Handle(CerrarLegalizacionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Nomina") && !_currentUser.IsInRole("Admin"))
            return Result<LegalizacionDetalleDto>.Failure("FORBIDDEN", "Solo nómina puede cerrar legalizaciones.");

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _unitOfWork,
            l => l.Cerrar(_currentUser.UserId),
            cancellationToken);
    }
}

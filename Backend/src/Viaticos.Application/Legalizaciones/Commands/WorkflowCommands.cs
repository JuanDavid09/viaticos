using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Application.Legalizaciones.Services;
using Viaticos.Application.Notificaciones;
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
        ILegalizacionDetalleFactory detalleFactory,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository,
        ICurrentUserService currentUser,
        Action<Legalizacion> action,
        string eventoNotificacion,
        string? detalle,
        CancellationToken cancellationToken)
    {
        var legalizacion = await legalizacionRepository.GetByIdAsync(legalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        try
        {
            action(legalizacion);
            await legalizacionRepository.PersistWorkflowTransitionAsync(legalizacion, cancellationToken);

            var updated = await legalizacionRepository.GetByIdAsync(legalizacionId, cancellationToken);
            var empleado = await empleadoRepository.GetByIdAsync(updated!.EmpleadoId, cancellationToken);
            var empleadoNombre = empleado is null
                ? "Empleado"
                : $"{empleado.Nombre} {empleado.Apellido}".Trim();

            await notificacionService.NotificarTransicionWorkflowAsync(
                updated,
                empleadoNombre,
                eventoNotificacion,
                currentUser.UserId,
                detalle,
                cancellationToken);

            var soportes = await documentoRepository.ListSoportesByGastoIdsAsync(
                updated.Gastos.Select(g => g.Id),
                cancellationToken);

            return Result<LegalizacionDetalleDto>.Success(
                await detalleFactory.CreateAsync(updated, soportes, cancellationToken));
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
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public EnviarValidacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.EnviarValidacion(_currentUser.UserId),
            NotificacionTipos.EnviadaValidacion,
            null,
            cancellationToken);
    }
}

public class EnviarAprobacionCommandHandler : IRequestHandler<EnviarAprobacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public EnviarAprobacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.EnviarAprobacion(_currentUser.UserId),
            NotificacionTipos.EnviadaAprobacion,
            null,
            cancellationToken);
    }
}

public class AprobarLegalizacionCommandHandler : IRequestHandler<AprobarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public AprobarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.Aprobar(_currentUser.UserId),
            NotificacionTipos.Aprobada,
            null,
            cancellationToken);
    }
}

public class RechazarLegalizacionCommandHandler : IRequestHandler<RechazarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public RechazarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.Rechazar(_currentUser.UserId, request.Comentario),
            NotificacionTipos.Rechazada,
            request.Comentario,
            cancellationToken);
    }
}

public class ReabrirLegalizacionCommandHandler : IRequestHandler<ReabrirLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public ReabrirLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.Reabrir(_currentUser.UserId),
            NotificacionTipos.Reabierta,
            null,
            cancellationToken);
    }
}

public class EnviarNominaCommandHandler : IRequestHandler<EnviarNominaCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public EnviarNominaCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ILegalizacionWorkflowService workflow,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _workflow = workflow;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
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
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.EnviarNomina(_currentUser.UserId),
            NotificacionTipos.EnviadaNomina,
            null,
            cancellationToken);
    }
}

public class CerrarLegalizacionCommandHandler : IRequestHandler<CerrarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionService _notificacionService;
    private readonly IEmpleadoRepository _empleadoRepository;

    public CerrarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IDocumentoRepository documentoRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificacionService notificacionService,
        IEmpleadoRepository empleadoRepository)
    {
        _legalizacionRepository = legalizacionRepository;
        _documentoRepository = documentoRepository;
        _detalleFactory = detalleFactory;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificacionService = notificacionService;
        _empleadoRepository = empleadoRepository;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(CerrarLegalizacionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole("Nomina") && !_currentUser.IsInRole("Admin"))
            return Result<LegalizacionDetalleDto>.Failure("FORBIDDEN", "Solo nómina puede cerrar legalizaciones.");

        return await WorkflowCommandHelper.ExecuteAsync(
            request.LegalizacionId,
            _legalizacionRepository,
            _documentoRepository,
            _detalleFactory,
            _unitOfWork,
            _notificacionService,
            _empleadoRepository,
            _currentUser,
            l => l.Cerrar(_currentUser.UserId),
            NotificacionTipos.Cerrada,
            null,
            cancellationToken);
    }
}

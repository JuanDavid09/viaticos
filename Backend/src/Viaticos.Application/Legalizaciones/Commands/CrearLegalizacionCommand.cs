using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Application.Legalizaciones.Services;
using Viaticos.Domain.Common;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.Commands;

public record CrearLegalizacionCommand(
    string Motivo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string? Destino,
    Guid? EmpleadoId = null) : IRequest<Result<LegalizacionDetalleDto>>;

public class CrearLegalizacionCommandValidator : AbstractValidator<CrearLegalizacionCommand>
{
    public CrearLegalizacionCommandValidator()
    {
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FechaFin).GreaterThanOrEqualTo(x => x.FechaInicio);
        RuleFor(x => x.MonedaId).NotEmpty();
        RuleFor(x => x.MontoAnticipo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Destino).MaximumLength(200);
        RuleFor(x => x.EmpleadoId).NotEqual(Guid.Empty);
    }
}

public class CrearLegalizacionCommandHandler : IRequestHandler<CrearLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly ILegalizacionWorkflowService _workflow;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly INotificacionService _notificacionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CrearLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IEmpleadoRepository empleadoRepository,
        ILegalizacionWorkflowService workflow,
        ILegalizacionDetalleFactory detalleFactory,
        INotificacionService notificacionService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _empleadoRepository = empleadoRepository;
        _workflow = workflow;
        _detalleFactory = detalleFactory;
        _notificacionService = notificacionService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(CrearLegalizacionCommand request, CancellationToken cancellationToken)
    {
        var targetEmpleadoId = request.EmpleadoId ?? _currentUser.UserId;
        var auth = await ResolveTargetEmpleadoAsync(targetEmpleadoId, cancellationToken);
        if (!auth.IsSuccess)
            return Result<LegalizacionDetalleDto>.Failure(auth.ErrorCode!, auth.Error!);

        var targetEmpleado = auth.Value!;

        try
        {
            var legalizacion = Legalizacion.Crear(
                targetEmpleadoId,
                request.Motivo,
                request.FechaInicio,
                request.FechaFin,
                request.MonedaId,
                request.MontoAnticipo,
                _currentUser.UserId,
                request.Destino);

            await _legalizacionRepository.AddAsync(legalizacion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var persisted = await _legalizacionRepository.GetByIdAsync(legalizacion.Id, cancellationToken);
            var empleadoNombre = $"{targetEmpleado.Nombre} {targetEmpleado.Apellido}".Trim();

            await _notificacionService.NotificarLegalizacionCreadaAsync(
                persisted!,
                empleadoNombre,
                _currentUser.UserId,
                cancellationToken);

            return Result<LegalizacionDetalleDto>.Success(
                await _detalleFactory.CreateAsync(persisted!, cancellationToken: cancellationToken));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }

    private async Task<Result<Domain.Core.Entities.Empleado>> ResolveTargetEmpleadoAsync(
        Guid targetEmpleadoId,
        CancellationToken cancellationToken)
    {
        var targetEmpleado = await _empleadoRepository.GetByIdAsync(targetEmpleadoId, cancellationToken);
        if (targetEmpleado is null)
            return Result<Domain.Core.Entities.Empleado>.Failure("NOT_FOUND", "Empleado no encontrado.");

        if (targetEmpleadoId == _currentUser.UserId)
            return Result<Domain.Core.Entities.Empleado>.Success(targetEmpleado);

        if (_currentUser.IsInRole("Admin"))
            return Result<Domain.Core.Entities.Empleado>.Success(targetEmpleado);

        if (_currentUser.IsInRole("JefeAprobador"))
        {
            if (targetEmpleado.JefeId != _currentUser.UserId)
            {
                return Result<Domain.Core.Entities.Empleado>.Failure(
                    "FORBIDDEN",
                    "Solo puede crear legalizaciones para empleados de su equipo.");
            }

            return Result<Domain.Core.Entities.Empleado>.Success(targetEmpleado);
        }

        return Result<Domain.Core.Entities.Empleado>.Failure(
            "FORBIDDEN",
            "No tiene permiso para crear legalizaciones para otro empleado.");
    }
}

using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Domain.Common;
using Viaticos.Domain.Legalizaciones.Entities;

namespace Viaticos.Application.Legalizaciones.Commands;

public record CrearLegalizacionCommand(
    string Motivo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string? Destino) : IRequest<Result<LegalizacionDetalleDto>>;

public class CrearLegalizacionCommandValidator : AbstractValidator<CrearLegalizacionCommand>
{
    public CrearLegalizacionCommandValidator()
    {
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FechaFin).GreaterThanOrEqualTo(x => x.FechaInicio);
        RuleFor(x => x.MonedaId).NotEmpty();
        RuleFor(x => x.MontoAnticipo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Destino).MaximumLength(200);
    }
}

public class CrearLegalizacionCommandHandler : IRequestHandler<CrearLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly INotificacionService _notificacionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CrearLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        IEmpleadoRepository empleadoRepository,
        INotificacionService notificacionService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _empleadoRepository = empleadoRepository;
        _notificacionService = notificacionService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(CrearLegalizacionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var legalizacion = Legalizacion.Crear(
                _currentUser.UserId,
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
            var empleado = await _empleadoRepository.GetByIdAsync(_currentUser.UserId, cancellationToken);
            var empleadoNombre = empleado is null
                ? "Empleado"
                : $"{empleado.Nombre} {empleado.Apellido}".Trim();

            await _notificacionService.NotificarLegalizacionCreadaAsync(
                persisted!,
                empleadoNombre,
                _currentUser.UserId,
                cancellationToken);

            return Result<LegalizacionDetalleDto>.Success(LegalizacionMapper.ToDetalle(persisted!));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

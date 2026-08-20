using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Application.Legalizaciones.Services;
using Viaticos.Domain.Common;

namespace Viaticos.Application.Legalizaciones.Commands;

public record ActualizarLegalizacionCommand(
    Guid Id,
    string Motivo,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid MonedaId,
    decimal MontoAnticipo,
    string? Destino) : IRequest<Result<LegalizacionDetalleDto>>;

public class ActualizarLegalizacionCommandValidator : AbstractValidator<ActualizarLegalizacionCommand>
{
    public ActualizarLegalizacionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FechaFin).GreaterThanOrEqualTo(x => x.FechaInicio);
        RuleFor(x => x.MonedaId).NotEmpty();
        RuleFor(x => x.MontoAnticipo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Destino).MaximumLength(200);
    }
}

public class ActualizarLegalizacionCommandHandler : IRequestHandler<ActualizarLegalizacionCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ILegalizacionDetalleFactory _detalleFactory;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        ILegalizacionDetalleFactory detalleFactory,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _detalleFactory = detalleFactory;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(ActualizarLegalizacionCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<LegalizacionDetalleDto>.Failure("FORBIDDEN", "No tiene permiso para modificar esta legalización.");

        try
        {
            legalizacion.Actualizar(
                request.Motivo,
                request.FechaInicio,
                request.FechaFin,
                request.MonedaId,
                request.MontoAnticipo,
                _currentUser.UserId,
                request.Destino);

            _legalizacionRepository.Update(legalizacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _legalizacionRepository.GetByIdAsync(request.Id, cancellationToken);
            return Result<LegalizacionDetalleDto>.Success(
                await _detalleFactory.CreateAsync(updated!, cancellationToken: cancellationToken));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

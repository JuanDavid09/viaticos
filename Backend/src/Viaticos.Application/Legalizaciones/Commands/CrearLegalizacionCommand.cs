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
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CrearLegalizacionCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
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
            return Result<LegalizacionDetalleDto>.Success(LegalizacionMapper.ToDetalle(persisted!));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

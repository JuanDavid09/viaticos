using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Application.Legalizaciones.DTOs;
using Viaticos.Domain.Common;

namespace Viaticos.Application.Legalizaciones.Commands;

public record AgregarGastoCommand(
    Guid LegalizacionId,
    Guid CategoriaGastoId,
    DateOnly FechaGasto,
    string Descripcion,
    decimal Monto,
    string? Proveedor,
    string? NumeroDocumento) : IRequest<Result<LegalizacionDetalleDto>>;

public class AgregarGastoCommandValidator : AbstractValidator<AgregarGastoCommand>
{
    public AgregarGastoCommandValidator()
    {
        RuleFor(x => x.LegalizacionId).NotEmpty();
        RuleFor(x => x.CategoriaGastoId).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Monto).GreaterThan(0);
        RuleFor(x => x.Proveedor).MaximumLength(200);
        RuleFor(x => x.NumeroDocumento).MaximumLength(50);
    }
}

public class AgregarGastoCommandHandler : IRequestHandler<AgregarGastoCommand, Result<LegalizacionDetalleDto>>
{
    private readonly ILegalizacionRepository _legalizacionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarGastoCommandHandler(
        ILegalizacionRepository legalizacionRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _legalizacionRepository = legalizacionRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LegalizacionDetalleDto>> Handle(AgregarGastoCommand request, CancellationToken cancellationToken)
    {
        var legalizacion = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
        if (legalizacion is null)
            return Result<LegalizacionDetalleDto>.Failure("NOT_FOUND", "Legalización no encontrada.");

        if (legalizacion.EmpleadoId != _currentUser.UserId)
            return Result<LegalizacionDetalleDto>.Failure("FORBIDDEN", "No tiene permiso para modificar esta legalización.");

        try
        {
            legalizacion.AgregarGasto(
                request.CategoriaGastoId,
                request.FechaGasto,
                request.Descripcion,
                request.Monto,
                _currentUser.UserId,
                request.Proveedor,
                request.NumeroDocumento);

            _legalizacionRepository.Update(legalizacion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _legalizacionRepository.GetByIdAsync(request.LegalizacionId, cancellationToken);
            return Result<LegalizacionDetalleDto>.Success(LegalizacionMapper.ToDetalle(updated!));
        }
        catch (DomainException ex)
        {
            return Result<LegalizacionDetalleDto>.Failure(ex.Code, ex.Message);
        }
    }
}

using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Core.Entities;

namespace Viaticos.Application.Empleados.Commands;

public record ActualizarEmpleadoCommand(
    Guid Id,
    string Nombre,
    string Apellido,
    string Rol,
    string? Departamento,
    Guid? JefeId,
    bool Activo) : IRequest<Result<EmpleadoDto>>;

public class ActualizarEmpleadoCommandValidator : AbstractValidator<ActualizarEmpleadoCommand>
{
    public ActualizarEmpleadoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rol).NotEmpty();
        RuleFor(x => x.Departamento).MaximumLength(100);
    }
}

public class ActualizarEmpleadoCommandHandler : IRequestHandler<ActualizarEmpleadoCommand, Result<EmpleadoDto>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarEmpleadoCommandHandler(IEmpleadoRepository empleadoRepository, IUnitOfWork unitOfWork)
    {
        _empleadoRepository = empleadoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmpleadoDto>> Handle(ActualizarEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByIdIncludingInactiveAsync(request.Id, cancellationToken);
        if (empleado is null)
            return Result<EmpleadoDto>.Failure("NOT_FOUND", "Usuario no encontrado.");

        try
        {
            var rol = EmpleadoMapper.ParseRol(request.Rol);
            var jefeId = EmpleadoJefeRules.ResolveJefeId(rol, request.JefeId);

            Empleado? jefe = null;
            if (jefeId.HasValue)
            {
                jefe = await _empleadoRepository.GetByIdAsync(jefeId.Value, cancellationToken);
            }

            var jefeValidation = EmpleadoJefeRules.ValidateJefeAssignment(
                rol,
                empleado.Id,
                jefeId,
                jefe);
            if (!jefeValidation.IsSuccess)
                return Result<EmpleadoDto>.Failure(jefeValidation.ErrorCode!, jefeValidation.Error!);

            empleado.ActualizarPerfil(request.Nombre, request.Apellido, rol, request.Departamento, jefeId);
        }
        catch (ArgumentException ex)
        {
            return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", ex.Message);
        }

        if (request.Activo)
            empleado.Activar();
        else
            empleado.Desactivar();

        _empleadoRepository.Update(empleado);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EmpleadoDto>.Success(EmpleadoMapper.ToDto(empleado));
    }
}

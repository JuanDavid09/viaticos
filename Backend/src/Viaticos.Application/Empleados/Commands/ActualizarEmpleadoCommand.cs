using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;

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
            empleado.ActualizarPerfil(request.Nombre, request.Apellido, rol, request.Departamento, request.JefeId);
        }
        catch (ArgumentException ex)
        {
            return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", ex.Message);
        }

        if (request.JefeId.HasValue)
        {
            if (request.JefeId.Value == empleado.Id)
                return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", "Un usuario no puede ser su propio jefe.");

            var jefe = await _empleadoRepository.GetByIdAsync(request.JefeId.Value, cancellationToken);
            if (jefe is null)
                return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", "El jefe indicado no existe o está inactivo.");
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

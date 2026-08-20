using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Core.Entities;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Application.Empleados.Commands;

public record CrearEmpleadoCommand(
    string CodigoEmpleado,
    string Email,
    string Nombre,
    string Apellido,
    string Rol,
    string PasswordTemporal,
    string? Departamento,
    Guid? JefeId) : IRequest<Result<EmpleadoDto>>;

public class CrearEmpleadoCommandValidator : AbstractValidator<CrearEmpleadoCommand>
{
    public CrearEmpleadoCommandValidator()
    {
        RuleFor(x => x.CodigoEmpleado).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rol).NotEmpty();
        RuleFor(x => x.PasswordTemporal).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Departamento).MaximumLength(100);
    }
}

public class CrearEmpleadoCommandHandler : IRequestHandler<CrearEmpleadoCommand, Result<EmpleadoDto>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CrearEmpleadoCommandHandler(
        IEmpleadoRepository empleadoRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _empleadoRepository = empleadoRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmpleadoDto>> Handle(CrearEmpleadoCommand request, CancellationToken cancellationToken)
    {
        if (await _empleadoRepository.ExistsByEmailAsync(request.Email, cancellationToken: cancellationToken))
            return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", "Ya existe un usuario con ese correo.");

        if (await _empleadoRepository.ExistsByCodigoAsync(request.CodigoEmpleado, cancellationToken: cancellationToken))
            return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", "Ya existe un usuario con ese código.");

        Rol rol;
        try
        {
            rol = EmpleadoMapper.ParseRol(request.Rol);
        }
        catch (ArgumentException ex)
        {
            return Result<EmpleadoDto>.Failure("VALIDATION_ERROR", ex.Message);
        }

        Empleado? jefe = null;
        if (request.JefeId.HasValue)
        {
            jefe = await _empleadoRepository.GetByIdAsync(request.JefeId.Value, cancellationToken);
        }

        var jefeValidation = EmpleadoJefeRules.ValidateJefeAssignment(
            rol,
            Guid.Empty,
            EmpleadoJefeRules.ResolveJefeId(rol, request.JefeId),
            jefe);
        if (!jefeValidation.IsSuccess)
            return Result<EmpleadoDto>.Failure(jefeValidation.ErrorCode!, jefeValidation.Error!);

        var passwordHash = _passwordHasher.HashPassword(request.PasswordTemporal);
        var empleado = Empleado.Crear(
            request.CodigoEmpleado,
            request.Email,
            request.Nombre,
            request.Apellido,
            rol,
            passwordHash,
            mustChangePassword: true,
            request.Departamento,
            EmpleadoJefeRules.ResolveJefeId(rol, request.JefeId));

        await _empleadoRepository.AddAsync(empleado, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EmpleadoDto>.Success(EmpleadoMapper.ToDto(empleado));
    }
}

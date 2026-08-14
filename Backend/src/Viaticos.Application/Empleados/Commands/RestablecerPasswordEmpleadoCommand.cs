using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;

namespace Viaticos.Application.Empleados.Commands;

public record RestablecerPasswordEmpleadoCommand(Guid Id, string PasswordTemporal)
    : IRequest<Result<EmpleadoDto>>;

public class RestablecerPasswordEmpleadoCommandValidator : AbstractValidator<RestablecerPasswordEmpleadoCommand>
{
    public RestablecerPasswordEmpleadoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PasswordTemporal).NotEmpty().MinimumLength(8);
    }
}

public class RestablecerPasswordEmpleadoCommandHandler
    : IRequestHandler<RestablecerPasswordEmpleadoCommand, Result<EmpleadoDto>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RestablecerPasswordEmpleadoCommandHandler(
        IEmpleadoRepository empleadoRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _empleadoRepository = empleadoRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmpleadoDto>> Handle(
        RestablecerPasswordEmpleadoCommand request,
        CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByIdIncludingInactiveAsync(request.Id, cancellationToken);
        if (empleado is null)
            return Result<EmpleadoDto>.Failure("NOT_FOUND", "Usuario no encontrado.");

        var passwordHash = _passwordHasher.HashPassword(request.PasswordTemporal);
        empleado.EstablecerPassword(passwordHash, mustChangePassword: true);

        _empleadoRepository.Update(empleado);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EmpleadoDto>.Success(EmpleadoMapper.ToDto(empleado));
    }
}

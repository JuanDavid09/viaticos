using FluentValidation;
using MediatR;
using Viaticos.Application.Auth;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Application.Auth.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword)
    : IRequest<Result<LoginResponse>>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Must(PasswordRules.IsStrongEnough)
            .WithMessage($"La nueva contraseña debe tener al menos {PasswordRules.MinLength} caracteres, una mayúscula, una minúscula y un número.");
        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage("La nueva contraseña debe ser diferente a la actual.");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<LoginResponse>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IEmpleadoRepository empleadoRepository,
        ICurrentUserService currentUser,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _empleadoRepository = empleadoRepository;
        _currentUser = currentUser;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(_currentUser.UserId, cancellationToken);
        if (empleado is null)
            return Result<LoginResponse>.Failure("NOT_FOUND", "Usuario no encontrado.");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, empleado.PasswordHash))
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "La contraseña actual no es correcta.");

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        empleado.CompletarCambioPassword(newHash);
        _empleadoRepository.Update(empleado);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rol = MapRol(empleado.Rol);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto,
            mustChangePassword: false);

        return Result<LoginResponse>.Success(new LoginResponse(
            token,
            expiresAt,
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto,
            MustChangePassword: false));
    }

    private static string MapRol(Rol rol) => rol switch
    {
        Rol.Empleado => "EMPLEADO",
        Rol.JefeAprobador => "JEFE_APROBADOR",
        Rol.Nomina => "NOMINA",
        Rol.Admin => "ADMIN",
        _ => "EMPLEADO"
    };
}

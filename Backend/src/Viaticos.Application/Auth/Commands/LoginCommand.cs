using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;
using Viaticos.Domain.Core.Enums;

namespace Viaticos.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string Rol,
    string NombreCompleto,
    bool MustChangePassword);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IEmpleadoRepository empleadoRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher)
    {
        _empleadoRepository = empleadoRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (empleado is null || !_passwordHasher.VerifyPassword(request.Password, empleado.PasswordHash))
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "Credenciales inválidas.");

        var rol = MapRol(empleado.Rol);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto,
            empleado.MustChangePassword);

        return Result<LoginResponse>.Success(new LoginResponse(
            token,
            expiresAt,
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto,
            empleado.MustChangePassword));
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

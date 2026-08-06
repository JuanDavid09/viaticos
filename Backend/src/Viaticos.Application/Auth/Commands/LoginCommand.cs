using FluentValidation;
using MediatR;
using Viaticos.Application.Common.Interfaces;
using Viaticos.Application.Common.Models;

namespace Viaticos.Application.Auth.Commands;

public record LoginCommand(string Email) : IRequest<Result<LoginResponse>>;

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    string Rol,
    string NombreCompleto);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(IEmpleadoRepository empleadoRepository, IJwtTokenService jwtTokenService)
    {
        _empleadoRepository = empleadoRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _empleadoRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (empleado is null)
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "Email no registrado o usuario inactivo.");

        var rol = MapRol(empleado.Rol);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto);

        return Result<LoginResponse>.Success(new LoginResponse(
            token,
            expiresAt,
            empleado.Id,
            empleado.Email,
            rol,
            empleado.NombreCompleto));
    }

    private static string MapRol(Domain.Core.Enums.Rol rol) => rol switch
    {
        Domain.Core.Enums.Rol.Empleado => "EMPLEADO",
        Domain.Core.Enums.Rol.JefeAprobador => "JEFE_APROBADOR",
        Domain.Core.Enums.Rol.Nomina => "NOMINA",
        Domain.Core.Enums.Rol.Admin => "ADMIN",
        _ => "EMPLEADO"
    };
}
